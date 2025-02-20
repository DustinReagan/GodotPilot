from grpc_tools import protoc
import sys
import os

# generate python grpc code from proto file
protoc.main((
    '',
    '-I../Grpc/Protos',  # this directory now contains both NodeTools.proto and google/protobuf/descriptor.proto
    '--python_out=.',
    '--grpc_python_out=.',
    '../Grpc/Protos/NodeTools.proto',
))
