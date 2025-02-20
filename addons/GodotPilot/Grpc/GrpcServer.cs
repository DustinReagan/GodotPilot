using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Godot;
using GodotPilotPlugin.Tools;
namespace GodotPilotPlugin.Grpc;
// a lightweight grpc server using grpc.core
public class GrpcServer
{
    private Server? _server;
    private const int Port = 5001;

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        // initialize the server and register grpc services
        _server = new Server
        {
            Services = { NodeToolService.BindService(new NodeToolServiceImpl()) },
            Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
        };

        _server.Start();
        GD.Print($"grpc server is listening on port {Port}");

        // wait indefinitely until cancellation is requested
        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            GD.Print("grpc server cancellation requested");
        }
    }

    public async Task StopAsync()
    {
        if (_server != null)
        {
            // gracefully shutdown the server
            await _server.ShutdownAsync();
            _server = null;
        }
    }
}

// grpc service implementation based on the generated proto code
// this class provides all rpc methods that mirror INodeToolFunctions
public class NodeToolServiceImpl : NodeToolService.NodeToolServiceBase
{
    private readonly NodeTools _nodeTools;

    public NodeToolServiceImpl()
    {
        // create a single instance of NodeTools
        _nodeTools = new NodeTools();
    }

    public override async Task<NodesResponse> GetAllNodes(Empty request, ServerCallContext context)
    {
        var nodes = await _nodeTools.GetAllNodes();
        var response = new NodesResponse();
        response.Nodes.AddRange(nodes);
        return response;
    }

    public override async Task<NodesResponse> GetSelectedNodes(Empty request, ServerCallContext context)
    {
        var nodes = await _nodeTools.GetSelectedNodes();
        var response = new NodesResponse();
        response.Nodes.AddRange(nodes);
        return response;
    }

    public override async Task<NodesResponse> GetNodesWithName(NameRequest request, ServerCallContext context)
    {
        var nodes = await _nodeTools.GetNodesWithName(request.Name);
        var response = new NodesResponse();
        response.Nodes.AddRange(nodes);
        return response;
    }

    public override async Task<OperationResponse> CreateNode(CreateNodeRequest request, ServerCallContext context)
    {
        var result = await _nodeTools.CreateNode(request.TypeName, request.ParentPath, request.Name);
        return new OperationResponse { Result = result };
    }

    public override async Task<OperationResponse> DeleteNode(PathRequest request, ServerCallContext context)
    {
        var result = await _nodeTools.DeleteNode(request.Path);
        return new OperationResponse { Result = result };
    }

    public override async Task<OperationResponse> RenameNode(RenameNodeRequest request, ServerCallContext context)
    {
        var result = await _nodeTools.RenameNode(request.Path, request.NewName);
        return new OperationResponse { Result = result };
    }

    public override async Task<OperationResponse> ChangeNodeType(ChangeNodeTypeRequest request, ServerCallContext context)
    {
        var result = await _nodeTools.ChangeNodeType(request.Path, request.NewType);
        return new OperationResponse { Result = result };
    }

    public override async Task<OperationResponse> DuplicateNode(DuplicateNodeRequest request, ServerCallContext context)
    {
        var result = await _nodeTools.DuplicateNode(request.Path, request.NewName);
        return new OperationResponse { Result = result };
    }

    public override async Task<OperationResponse> SetNodeProperty(SetNodePropertyRequest request, ServerCallContext context)
    {
        var result = await _nodeTools.SetNodeProperty(request.Path, request.PropertyName, request.Value);
        return new OperationResponse { Result = result };
    }

    public override async Task<OperationResponse> SelectNodes(SelectNodesRequest request, ServerCallContext context)
    {
        var result = await _nodeTools.SelectNodes(request.Paths.ToArray());
        return new OperationResponse { Result = result };
    }

    public override async Task<OperationResponse> ReparentNodes(ReparentNodesRequest request, ServerCallContext context)
    {
        var result = await _nodeTools.ReparentNodes(request.Paths.ToArray(), request.ParentPath);
        return new OperationResponse { Result = result };
    }

    public override async Task<OperationResponse> CopyNodeProperties(CopyNodePropertiesRequest request, ServerCallContext context)
    {
        var result = await _nodeTools.CopyNodeProperties(request.SourcePath, request.TargetPaths.ToArray());
        return new OperationResponse { Result = result };
    }

    public override Task<NodeTypesResponse> GetNodeTypes(Empty request, ServerCallContext context)
    {
        var types = _nodeTools.GetNodeTypes();
        var response = new NodeTypesResponse();
        response.Types_.AddRange(types);
        return Task.FromResult(response);
    }

    public override async Task<NodesResponse> GetAllNodesByType(NodeTypeRequest request, ServerCallContext context)
    {
        var nodes = await _nodeTools.GetAllNodesByType(request.TypeName);
        var response = new NodesResponse();
        response.Nodes.AddRange(nodes);
        return response;
    }

    public override async Task<OperationResponse> CreateAnimation(CreateAnimationRequest request, ServerCallContext context)
    {
        var result = await _nodeTools.CreateAnimation(request.AnimatedSpritePath, request.AnimationName, request.Fps);
        return new OperationResponse { Result = result };
    }

    // add a new method for getting node properties
    public override async Task<PropertiesResponse> GetNodeProperties(PathRequest request, ServerCallContext context)
    {
        var properties = await _nodeTools.GetNodeProperties(request.Path);
        var response = new PropertiesResponse();
        foreach (var kv in properties)
        {
            response.Properties.Add(kv.Key, kv.Value);
        }
        return response;
    }

    public override async Task<OperationResponse> DeleteAnimation(DeleteAnimationRequest request, ServerCallContext context)
    {
        var result = await _nodeTools.DeleteAnimation(request.AnimatedSpritePath, request.AnimationName);
        return new OperationResponse { Result = result };
    }

    public override async Task<OperationResponse> SetAnimationSpeed(SetAnimationSpeedRequest request, ServerCallContext context)
    {
        var result = await _nodeTools.SetAnimationSpeed(request.AnimatedSpritePath, request.AnimationName, request.Fps);
        return new OperationResponse { Result = result };
    }

    public override async Task<OperationResponse> RenameAnimation(RenameAnimationRequest request, ServerCallContext context)
    {
        var result = await _nodeTools.RenameAnimation(request.AnimatedSpritePath, request.OldAnimationName, request.NewAnimationName);
        return new OperationResponse { Result = result };
    }

    public override async Task<AnimationNamesResponse> GetAnimationNames(PathRequest request, ServerCallContext context)
    {
        var names = await _nodeTools.GetAnimationNames(request.Path);
        var response = new AnimationNamesResponse();
        response.Names.AddRange(names);
        return response;
    }
}
