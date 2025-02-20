using Godot;
using System;

namespace GodotPilotPlugin.Tools;

public static class NodeAnimationToolsStatic
{


    public static string CreateAnimation(string animatedSpritePath, string animationName, float? fps = null)
    {
        var (node, error) = GetValidatedSprite(animatedSpritePath);
        if (error != null) return error;

        if (node!.SpriteFrames == null){
            node.SpriteFrames = new SpriteFrames();
        }
        var spriteFrames = node.SpriteFrames;

        if (spriteFrames!.HasAnimation(animationName))
        {
            return $"Animation '{animationName}' already exists for sprite at {animatedSpritePath}";
        }

        spriteFrames.AddAnimation(animationName);
        if( fps != null )
        {
            spriteFrames.SetAnimationSpeed(animationName, fps.Value);
        }

        return $"Successfully created animation '{animationName}' for sprite at {animatedSpritePath}";
    }

    public static string DeleteAnimation(string animatedSpritePath, string animationName)
    {
        var (spriteFrames, error) = GetValidatedSpriteFrames(animatedSpritePath);
        if (error != null) return error;

        if (!spriteFrames!.HasAnimation(animationName))
        {
            return $"Error: animation {animationName} does not exist on node at path {animatedSpritePath}";
        }

        spriteFrames.RemoveAnimation(animationName);
        return $"Successfully deleted animation {animationName} from node at path {animatedSpritePath}";
    }

    public static string SetAnimationSpeed(string animatedSpritePath, string animationName, float fps)
    {
        var (spriteFrames, error) = GetValidatedSpriteFrames(animatedSpritePath);
        if (error != null) return error;

        if (!spriteFrames!.HasAnimation(animationName))
        {
            return $"Error: animation {animationName} does not exist on node at path {animatedSpritePath} (call CreateAnimation to create an animation)";
        }

        spriteFrames.SetAnimationSpeed(animationName, fps);
        return $"Successfully set animation speed to {fps} fps for animation {animationName} on node at path {animatedSpritePath}";
    }

    public static string RenameAnimation(string animatedSpritePath, string oldAnimationName, string newAnimationName)
    {
        var (spriteFrames, error) = GetValidatedSpriteFrames(animatedSpritePath);
        if (error != null) return error;

        if (!spriteFrames!.HasAnimation(oldAnimationName))
        {
            return $"Error: animation {oldAnimationName} does not exist on node at path {animatedSpritePath}";
        }

        spriteFrames.RenameAnimation(oldAnimationName, newAnimationName);
        return $"Successfully renamed animation {oldAnimationName} to {newAnimationName} on node at path {animatedSpritePath}";
    }

    public static List<string> GetAnimationNames(string animatedSpritePath)
    {
        var (spriteFrames, error) = GetValidatedSpriteFrames(animatedSpritePath);
        if (error != null) return new List<string>{error};

        if (spriteFrames == null)
        {
            return new List<string>();
        }

        var animations = new List<string>();
        for (int i = 0; i < spriteFrames.GetAnimationNames().Length; i++)
        {
            animations.Add(spriteFrames.GetAnimationNames()[i]);
        }
        return animations;
    }

    private static (AnimatedSprite2D? sprite, string? error) GetValidatedSprite(string animatedSpritePath)
    {
        var root = EditorInterface.Singleton.GetEditedSceneRoot();
        var node = root.GetNode(animatedSpritePath);

        if (node == null)
        {
            return (null, $"Error: could not find node at path: {animatedSpritePath} (Call GetAllNodes to get a list of all nodes in the scene)");
        }

        if (node is not AnimatedSprite2D animatedSprite)
        {
            return (null, $"Error: node at path {animatedSpritePath} is not an AnimatedSprite2D. Call GetNodesByType to get a list of all AnimatedSprite2D nodes in the scene");
        }

        return (animatedSprite, null);
    }

    private static (SpriteFrames? spriteFrames, string? error) GetValidatedSpriteFrames(string animatedSpritePath)
    {
        var (sprite, error) = GetValidatedSprite(animatedSpritePath);
        if (error != null) return (null, error);

        var spriteFrames = sprite!.SpriteFrames;
        if (spriteFrames == null)
        {
            return (null, $"Error: node at path {animatedSpritePath} does not have a SpriteFrames (call CreateAnimation to create an animation)");
        }
        return (spriteFrames, null);
    }
}