$(function() {
    var data_types = ["card", "action", "tile", "tile_add_on"];//, "map"];

    var tools = {};

    function SetupWorkspace(tool, item, devider) {
        var $workspace = $(".tool_workspace");
        if (!devider) {
            $workspace.empty();
            $workspace.append("<div class='header'>" + tool.name + "</div>");
        } else {
            $workspace.append("<hr>");
        }

        Object.keys(tool.schema).forEach((key) => {
            var value = item[key];
            var enabled = value != undefined;

            $workspace.append('<span class="key">' + key + '</span>');
            $workspace.append('<input class="text" type="text" placeholder="' + value + '" value="' + value + '"/>');
            $workspace.append('<input class="check" type="checkbox" ' + (enabled ? "checked" : "uncheched") + '/>');
            $workspace.append('<br/>');
        });
    };

    $(data_types).each(function(i, tool_name) {
        var tool = {};
        tool.name = tool_name;
        var prototype_json = "../client/Assets/Resources/json/prototypes/" + tool_name + ".json";
        $.getJSON(prototype_json, function(json) {
            tool.prototype = json;
            console.log( "Loaded " + tool_name + " prototype." );
        });
        var schema_json = "../client/Assets/Resources/json/schema/" + tool_name + ".json";
        $.getJSON(schema_json, function(json) {
            tool.schema = json;
            console.log( "Loaded " + tool_name + " schema." );
        });
        var tool_json = "../client/Assets/Resources/json/" + tool_name + "s.json";
        $.getJSON(tool_json, function(json) {
            tool.instances = json;
            console.log( "Loaded " + tool_name + " instances." );
        });
        tool.button = $("<button class='tool'>" + tool_name + "</button>").click(() => {
            var tool = tools[tool_name];
            var more_than_one = false;
            var items = tool.instances[tool_name + "s"];
            Object.keys(items).forEach((key) => {
                SetupWorkspace(tool, items[key], more_than_one);
                more_than_one = true;
            })
        });

        tools[tool_name] = tool;
        $(".tool_list").append(tool.button);
    });
});