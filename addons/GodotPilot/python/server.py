from flask import Flask, request, jsonify
from flask.json.provider import DefaultJSONProvider
import grpc
import NodeTools_pb2
import NodeTools_pb2_grpc
from planner import get_agent
import logging
from langchain_core.messages import AIMessage, HumanMessage, ToolMessage
# Configure logging to output to console with timestamp
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
# After the existing basicConfig
logging.getLogger('generated_tools').propagate = True

app = Flask(__name__)


(agent, llm) = get_agent()
# create a grpc channel
#channel = grpc.insecure_channel('localhost:5001')
# create a stub (client)
#stub = NodeTools_pb2_grpc.NodeToolServiceStub(channel)

@app.route('/act', methods=['POST'])
def act():
    # get data from request body
    data = request.get_json()
    if not data or 'prompt' not in data or 'id' not in data:
        return jsonify(error="request body must contain both 'prompt' and 'id'"), 400

    try:
        llm.model = data['model']
        response = agent.invoke({"messages": [HumanMessage(content=data['prompt'])]}, config={"thread_id": data['id']})
        messages = []
        for message in response['messages']:
            if isinstance(message, AIMessage) or isinstance(message, ToolMessage):
                msg = {'type': message.type, 'content': message.content}
                if message.type == 'ai' and message.tool_calls:
                    msg['tool_calls'] = message.tool_calls
                messages.append(msg)
        # wrap messages in a dict and use default_serializer
        return jsonify(messages)
    except grpc.RpcError as e:
        return jsonify(error=str(e)), 500


def transform_message(message):
    m = { message.type : message.content}
    if message.type == 'ai' and message.tool_calls:
        m['tool_calls'] = message.tool_calls
    return m
if __name__ == '__main__':
    app.run(host='0.0.0.0', port=8082)