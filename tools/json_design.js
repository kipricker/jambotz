$(function() {
    var data_types = ["card", "action", "tile", "tile_add_on"];//, "map"];

    var tools = {};

    function SetupWorkspace(tool, item, devider) {
        var $workspace = $(".tool_space");
        if (!devider) {
            $workspace.empty();
            $(".tool_header").empty().append(tool.name + "s");
        } else {
            $workspace.append("<hr>");
        }

        Object.keys(tool.schema).forEach((key) => {
            var value = item[key];
            var enabled = value != undefined;
            if (value == undefined)
                value = tool.schema[key];

            var $item = $('<div class="variable"></div>');
            $item.append('<span class="key">' + key + '</span>');
            if (typeof tool.schema[key] == "boolean") {
                $item.append('<input class="text" type="checkbox" ' + (value ? "checked" : "unchecked") + " " + (enabled ? "enabled" : "disabled") + '/>');

            } else {
                $item.append('<input class="text" type="text" placeholder="' + value + '" value="' + value + '" ' + (enabled ? "enabled" : "disabled") + '/>');
            }
            $item.append($('<input class="check" type="checkbox" ' + (enabled ? "checked" : "unchecked") + '/>').change(function() {
                var enabled = $(this).prop( "checked" );
                $(this).siblings(".text").prop('disabled', !enabled);
            }));
            $item.append('<br/>');
            $workspace.append($item);
        });
    };

    function SetupMapSpace(map) {
        var $workspace = $(".tool_space");
        $workspace.empty();
        $(".tool_header").empty().append(map.name);
    
        // //
        // "x" : 0, "y" : 0,
        // "tile" : "tile_name",
        // "tile_add_ons" : [
        //     {
        //         "name" : "tile_add_on_name",
        //         "trigger" : {
        //             "name" : "trigger_link_name",
        //             "actions" : []
        //         },
        //         "edge" : "north",
        //         "start_state" : "default"
        //     }
        // ],
        // "orientation" : "vertical",
        // "variation" : "tile_variation"

        var map_layout = [];
        Object.values(map.map_data).forEach((tile) => {
            var cell = "<div class='tile'></div>";
            var laser = "<div class='laser'></div>";
            $(cell).append(laser);
            $(cell).prop("top", tile.x + "px");
            $(cell).prop("left", tile.y + "px");

            $(cell).addClass(tile.tile);

            Object.values(tile.tile_add_ons).forEach((add_on) => {
                switch(add_on.name) {
                    case "wall":
                        $(cell).addClass('tile_wall_' + add_on.edge);
                        break;
                    case "laser":
                        $(laser).addClass('tile_laser_' + add_on.edge);
                        break;
                }
            })
            $workspace.append(cell);
        });
        
        Object.keys(tool.schema).forEach((key) => {
            var value = item[key];
            var enabled = value != undefined;
            if (value == undefined)
                value = tool.schema[key];

            var $item = $('<div class="variable"></div>');
            $item.append('<span class="key">' + key + '</span>');
            if (typeof tool.schema[key] == "boolean") {
                $item.append('<input class="text" type="checkbox" ' + (value ? "checked" : "unchecked") + " " + (enabled ? "enabled" : "disabled") + '/>');

            } else {
                $item.append('<input class="text" type="text" placeholder="' + value + '" value="' + value + '" ' + (enabled ? "enabled" : "disabled") + '/>');
            }
            $item.append($('<input class="check" type="checkbox" ' + (enabled ? "checked" : "unchecked") + '/>').change(function() {
                var enabled = $(this).prop( "checked" );
                $(this).siblings(".text").prop('disabled', !enabled);
            }));
            $item.append('<br/>');
            $workspace.append($item);
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
        $(".tools").append(tool.button);
    });

    var mapper = $("<select name='maps'><option></option></select>");

    var maps = {};
    var maps_json = "../client/Assets/Resources/json/maps.json";
    $.getJSON(maps_json, function(json) {
        var map_names = json.maps;
        console.log( "Loaded map names." );

        Object.values(map_names).forEach((map_name) => {
            var map_json = "../client/Assets/Resources/json/maps/" + map_name + ".json";
            $.getJSON(map_json, function(json) {
                if (json.name == undefined)
                    json.name = map_name;
                maps[map_name] = json;
                $(mapper).append("<option value='" + map_name + "'>" + json.name + "</option>");
            });
        });
    });

    $(mapper).change(function() {
        SetupMapSpace(maps[this.value]);
    });
    $(".tools").append(mapper);
});