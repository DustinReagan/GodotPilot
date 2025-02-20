from langchain_ollama import ChatOllama
from langgraph.checkpoint.memory import MemorySaver
from langgraph.prebuilt import create_react_agent
from auto_grpc_tools import get_tools
from generated_tools import tools as structured_tools
from langchain_openai.chat_models.base import BaseChatOpenAI
from dotenv import load_dotenv
import os
load_dotenv()

api_key = os.getenv('DEEPSEEK_API_KEY')
system_prompt = """
You are an assistant that can help with tasks in the Godot Editor v4.3 by calling the provided tools.
Choose the tool that is most appropriate to the task and parameterize it appropriately.
You may need to call multiple tools to complete the task.
For example, if you need to create a Node, you will need to call GetNodeTypes to help choose the type of node to create, then call GetAllNodes or GetSelectedNodes to help choose the parent node, then call CreateNode to actually create the node.
The only actions you should take are to call tools. Make a plan and then call tools to complete the task.
The root node path is always "."
Use English for all your messages, responses & tool calls.
"""
def get_agent():
    llm = BaseChatOpenAI(
        model='deepseek-chat',
        openai_api_key=api_key,
        openai_api_base='https://api.deepseek.com',
        max_tokens=1024
    )
    tools = structured_tools

    checkpointer = MemorySaver()
    agent_executor = create_react_agent(llm, tools, prompt=system_prompt, checkpointer=checkpointer, debug=True)
    return (agent_executor, llm)
