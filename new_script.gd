@tool
extends EditorScript

func list_nodes_by_path(node: Node, path_prefix: String = "") -> void:
	var current_path = path_prefix + "/" + node.name
	print(current_path)
	
	for child in node.get_children():
		if child is Node:
			list_nodes_by_path(child, current_path)

# Called when the script is executed (using File -> Run in Script Editor).
func _run() -> void:
	var root = get_editor_interface().get_edited_scene_root()
	var node = root.get_node('/root/Fred')
	print(node.name)
	
	#list_nodes_by_path(root)
