from langchain_ollama import ChatOllama
from langgraph.checkpoint.memory import MemorySaver
from langgraph.prebuilt import create_react_agent
from generated_tools import tools as structured_tools
from langchain_google_genai import ChatGoogleGenerativeAI
from dotenv import load_dotenv
load_dotenv()

system_prompt = """
You are an assistant that can help with tasks in the Godot Editor v4.3 by calling the provided tools.
Choose the tool that is most appropriate to the task and parameterize it appropriately.
You may need to call multiple tools to complete the task.
For example, if you need to create a Node, you will need to call GetNodeTypes to help choose the type of node to create, then call GetAllNodes or GetSelectedNodes to help choose the parent node, then call CreateNode to actually create the node.
The only actions you should take are to call tools. Make a plan and then call tools to complete the task.
The root node path is always "."
Any user-facing messages should be in syntactically correct Markdown format.
"""
def get_agent():
    llm = ChatGoogleGenerativeAI(
        model="gemini-2.0-flash-lite-preview-02-05",
        temperature=0,
        max_tokens=1024,
        timeout=None,
        max_retries=2,
    )
    tools = structured_tools

    checkpointer = MemorySaver()
    agent_executor = create_react_agent(llm, tools, prompt=system_prompt, checkpointer=checkpointer, debug=True)
    return (agent_executor, llm)
