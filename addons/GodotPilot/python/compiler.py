import operator
import re
from datetime import datetime
from typing import Annotated, TypedDict, Union, Tuple, List, Literal

from dotenv import load_dotenv
from langchain import hub
from langchain_core.tools import tool
from langchain_core.prompts import ChatPromptTemplate
from langchain_ollama import ChatOllama
from langgraph.checkpoint.memory import MemorySaver
from typing_extensions import TypedDict
from langgraph.prebuilt import create_react_agent
from pydantic import BaseModel, Field
from langchain_core.prompts import ChatPromptTemplate
from langgraph.graph import StateGraph, START, END
import os
from dotenv import load_dotenv
load_dotenv()

@tool
def get_now(format: str = "%Y-%m-%d %H:%M:%S"):
    """
    Get the current time
    Args:
        format: the format string for the datetime output
    """
    return datetime.now().strftime(format)

tools = [get_now]
llm = ChatOllama(model="llama3.2", base_url=os.getenv("OLLAMA_SERVER_URL"))
prompt = "You are a helpful assistant."
checkpointer = MemorySaver()
agent_executor = create_react_agent(llm, tools, prompt=prompt, checkpointer=checkpointer)
config = {"thread_id": "1"}
result = agent_executor.invoke({"messages": [("user", "What time is it?")]}, config=config)
print(result)
print('\n')
result = agent_executor.invoke({"messages": [("user", "what about now?")]}, config=config)
print(result)
# class PlanExecute(TypedDict):
#     input: str
#     plan: List[str]
#     past_steps: Annotated[List[Tuple], operator.add]
#     response: str

# class Plan(BaseModel):
#     """Plan to follow in future"""

#     steps: List[str] = Field(
#         description="different steps to follow, should be in sorted order"
#     )

# planner_prompt = ChatPromptTemplate.from_messages(
#     [
#         (
#             "system",
#             """For the given objective, come up with a simple step by step plan. \
# This plan should involve individual tasks, that if executed correctly will yield the correct answer. Do not add any superfluous steps. \
# The result of the final step should be the final answer. Make sure that each step has all the information needed - do not skip steps.""",
#         ),
#         ("placeholder", "{messages}"),
#     ]
# )
# planner = planner_prompt | ChatOllama(
#     model="qwen2.5:32b", temperature=0
# ).with_structured_output(Plan)

# class Response(BaseModel):
#     """Response to user."""

#     response: str


# class Act(BaseModel):
#     """Action to perform."""

#     action: Union[Response, Plan] = Field(
#         description="Action to perform. If you want to respond to user, use Response. "
#         "If you need to further use tools to get the answer, use Plan."
#     )


# replanner_prompt = ChatPromptTemplate.from_template(
#     """For the given objective, come up with a simple step by step plan. \
# This plan should involve individual tasks, that if executed correctly will yield the correct answer. Do not add any superfluous steps. \
# The result of the final step should be the final answer. Make sure that each step has all the information needed - do not skip steps.

# Your objective was this:
# {input}

# Your original plan was this:
# {plan}

# You have currently done the follow steps:
# {past_steps}

# Update your plan accordingly. If no more steps are needed and you can return to the user, then respond with that. Otherwise, fill out the plan. Only add steps to the plan that still NEED to be done. Do not return previously done steps as part of the plan."""
# )


# replanner = replanner_prompt | ChatOllama(
#     model="qwen2.5:32b", temperature=0
# ).with_structured_output(Act)

# async def execute_step(state: PlanExecute):
#     plan = state["plan"]
#     plan_str = "\n".join(f"{i+1}. {step}" for i, step in enumerate(plan))
#     task = plan[0]
#     task_formatted = f"""For the following plan:
# {plan_str}\n\nYou are tasked with executing step {1}, {task}."""
#     agent_response = await agent_executor.ainvoke(
#         {"messages": [("user", task_formatted)]}
#     )
#     return {
#         "past_steps": [(task, agent_response["messages"][-1].content)],
#     }


# async def plan_step(state: PlanExecute):
#     plan = await planner.ainvoke({"messages": [("user", state["input"])]})
#     return {"plan": plan.steps}


# async def replan_step(state: PlanExecute):
#     output = await replanner.ainvoke(state)
#     if isinstance(output.action, Response):
#         return {"response": output.action.response}
#     else:
#         return {"plan": output.action.steps}


# def should_end(state: PlanExecute):
#     if "response" in state and state["response"]:
#         return END
#     else:
#         return "agent"


# workflow = StateGraph(PlanExecute)

# # Add the plan node
# workflow.add_node("planner", plan_step)

# # Add the execution step
# workflow.add_node("agent", execute_step)

# # Add a replan node
# workflow.add_node("replan", replan_step)

# workflow.add_edge(START, "planner")

# # From plan we go to agent
# workflow.add_edge("planner", "agent")

# # From agent, we replan
# workflow.add_edge("agent", "replan")

# workflow.add_conditional_edges(
#     "replan",
#     # Next, we pass in the function that will determine which node is called next.
#     should_end,
#     ["agent", END],
# )

# # Finally, we compile it!
# app = workflow.compile()

# async def main():
#     config = {"recursion_limit": 50}
#     inputs = {"input": "what is the hometown of the mens 2024 Australia open winner?"}
#     async for event in app.astream(inputs, config=config):
#         for k, v in event.items():
#             if k != "__end__":
#                 print(v)

# if __name__ == "__main__":
#     import asyncio
#     asyncio.run(main())