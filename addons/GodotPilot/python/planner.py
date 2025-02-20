from langchain_ollama import ChatOllama
from langgraph.checkpoint.memory import MemorySaver
from langgraph.prebuilt import create_react_agent
from auto_grpc_tools import get_tools
from generated_tools import tools as structured_tools
import json

system_prompt = """
You are a helpful assistant that can help with tasks in the Godot Editor by calling the provided tools.
Choose the tool that is most appropriate to the task and parameterize it appropriately.
You may need to call multiple tools to complete the task.
For example, if you need to create a Node, you will need to call GetNodeTypes to help choose the type of node to create, then call GetAllNodes or GetSelectedNodes to help choose the parent node, then call CreateNode to actually create the node.
The only actions you should take are to call tools. Make a plan and then call tools to complete the task.
The root node path is always "."
Use English for all your messages, responses & tool calls.
"""
def get_agent():
    tools = structured_tools
    llm = ChatOllama(model="qwen2.5:32b" ,base_url="http://192.168.0.96:11434")
    prompt = system_prompt
    checkpointer = MemorySaver()
    agent_executor = create_react_agent(llm, tools, prompt=prompt, checkpointer=checkpointer, debug=True)
    return (agent_executor, llm)

    # result = agent_executor.invoke({"messages": [("user", "what about now?")]}, config=config)
    # print(result)
def default_serializer(o):
    # if the object has a to_json method, try to use it
    if hasattr(o, "to_json"):
        try:
            return json.loads(o.to_json())
        except Exception:
            # fallback using the object's __dict__ attribute as a dictionary representation
            return o.__dict__
    # if the object has a __dict__ attribute, return that
    elif hasattr(o, "__dict__"):
        return o.__dict__
    # otherwise, return the string representation of the object
    return str(o)
if __name__ == "__main__":
    agent = get_agent()
    response = agent.invoke({"messages": [("user", "List all the nodes in the scene.")]}, config={"thread_id": "1"})
    print(json.dumps(response,default=default_serializer, indent=4))
    for message in response['messages']:
        print(message.type + ": ", message.content)
