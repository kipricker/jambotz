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


    var global_brush = "blank";

    function SetupMapSpace(map, map_name) {

        var map_layout = [];

        var $workspace = $(".tool_space");
        $workspace.empty();
        $(".tool_header").empty().append(map.name);

        var $map_space = $("<div class='map_space'></div>");

        var $blank_brush = $("<button class='brushes' value='blank'>Blank</button>");
        var $conveyour_brush = $("<button class='brushes' value='conveyour'>Conveyour</button>");
        var $laser_brush = $("<button value='laser'>Laser</button>");
        var $wall_brush = $("<button value='wall'>Wall</button>");

        
        var $save = $("<button>Save</button>").click(() => {
            var new_map = map;
            new_map.map_data = [];
            for (i=0; i < new_map.width; i++) {
                for (j=0; j < new_map.height; j++) {
                    var cell = map_layout[i][j];
                    var type = "";
                    if (cell.hasClass("tile_blank")) {
                        type = "blank";
                    } else if (cell.hasClass("tile_conveyour")) {
                        type = "conveyour";   
                    } else if (cell.hasClass("tile_pusher")) {
                        type = "pusher";
                    }

                    if (type != "") {
                        var new_tile = {
                            "tile" : type,
                            "x" : i,
                            "y" : j,
                            "tile_add_ons" : []
                        };

                        if (type != "blank") {
                            var orientation = cell.find(".orientation");

                            if (orientation.hasClass("tile_orientation_north")){
                                new_tile.orientation = "north";
                            } else if (orientation.hasClass("tile_orientation_south")){
                                new_tile.orientation = "south";
                            } else if (orientation.hasClass("tile_orientation_east")){
                                new_tile.orientation = "east";
                            } else if (orientation.hasClass("tile_orientation_west")){
                                new_tile.orientation = "west";
                            }
                        }

                        if (cell.hasClass("tile_wall_north")){
                            new_tile.tile_add_ons.push({
                                "name" : "wall",
                                "edge" : "north"
                            })
                        }
                        if (cell.hasClass("tile_wall_south")){
                            new_tile.tile_add_ons.push({
                                "name" : "wall",
                                "edge" : "south"
                            })
                        }
                        if (cell.hasClass("tile_wall_east")){
                            new_tile.tile_add_ons.push({
                                "name" : "wall",
                                "edge" : "east"
                            })
                        }
                        if (cell.hasClass("tile_wall_west")){
                            new_tile.tile_add_ons.push({
                                "name" : "wall",
                                "edge" : "west"
                            })
                        }

                        // Laser
                        var laser = cell.find(".laser");

                        if (laser.hasClass("tile_laser_north")){
                            new_tile.tile_add_ons.push({
                                "name" : "laser",
                                "edge" : "north"
                            })
                        }
                        if (laser.hasClass("tile_laser_south")){
                            new_tile.tile_add_ons.push({
                                "name" : "laser",
                                "edge" : "south"
                            })
                        }
                        if (laser.hasClass("tile_laser_east")){
                            new_tile.tile_add_ons.push({
                                "name" : "laser",
                                "edge" : "east"
                            })
                        }
                        if (laser.hasClass("tile_laser_west")){
                            new_tile.tile_add_ons.push({
                                "name" : "laser",
                                "edge" : "west"
                            })
                        }

                        new_map.map_data.push(new_tile);
                    }
                }
            }

            var json = JSON.stringify(new_map);
            var blob = new Blob([json], {type: "application/json"});
            var url  = URL.createObjectURL(blob);

            var anchor = document.createElement('a');
            anchor.href = url;
            anchor.download = map_name+ ".json";
            anchor.click();
        });

        $workspace.append($blank_brush);
        $workspace.append($conveyour_brush);

        $(".brushes").click(function() {
            global_brush = $(this).val();
        });

        $workspace.append($save);
        
        $workspace.append($map_space);

        var unit = 34;
        for(i=0; i < map.height; i++) {
            var row = [];
            for(j=0; j < map.width; j++) {
                var cell = $("<div class='tile'></div>").css("top", (i * unit) + "pt").css("left", (j * unit) + "pt");
                var orientation = $("<div class='orientation'></div>");
                var laser = $("<div class='laser'></div>");
                cell.append(laser);
                laser.append(orientation);

                row.push(cell);
                $map_space.append(cell);
            }
            map_layout.push(row);
        }

        $(".tile").click((tile) => {
            var type = "";
            var $this = $(tile.target);
            if ($this.hasClass("tile_blank")) {
                type = "blank";
            } else if ($this.hasClass("tile_conveyour")) {
                type = "conveyour";   
            } else if ($this.hasClass("tile_pusher")) {
                type = "pusher";
            }

            $this.removeClass("tile_blank");
            $this.removeClass("tile_conveyour");
            $this.removeClass("tile_pusher");

            switch(type) {
                case "blank":
                case "conveyour":
                case "pusher":
                    type = "";
                    break;
                case "":
                    type = global_brush;
                    break;
            }

            if (type != "")
                $this.addClass("tile_" + type);
        });

        $map_space.contextMenu({
            selector: '.tile', 
            items: {
                tile_type: {
                    name: "Tile Type",
                    items: {
                        blank : {
                            name: "Blank",
                            type: "radio",
                            radio: "tile_type",
                            value: "blank",
                        },
                        pusher : {
                            name: "Pusher",
                            type: "radio",
                            radio: "tile_type",
                            value: "pusher",
                        },
                        conveyour : {
                            name: "Conveyour",
                            type: "radio",
                            radio: "tile_type",
                            value: "conveyour",
                        },
                        orientation: {
                            name: "Direction",
                            type: "select",
                            options: {"north" : "north", "south":"south", "east":"east", "west":"west"},
                            visible: function(key, opt) {
                                return !opt.inputs.blank.$input.prop("checked");
                            }
                        }
                    }
                },
                walls: {
                    name: "Walls",
                    items: {
                        wall_north: {
                            name: "North",
                            type: "checkbox",
                            value: "north"
                        },
                        wall_south: {
                            name: "South",
                            type: "checkbox",
                            value: "south"
                        },
                        wall_east: {
                            name: "East",
                            type: "checkbox",
                            value: "east"
                        },
                        wall_west: {
                            name: "West",
                            type: "checkbox",
                            value: "west"
                        }
                    }
                },
                lasers: {
                    name: "Lasers",
                    items: {
                        laser_north: {
                            name: "North",
                            type: "checkbox",
                            value: "north",
                            disabled: function(key, opt){
                                return !opt.inputs.wall_north.$input.prop("checked");
                            }
                        },
                        laser_south: {
                            name: "South",
                            type: "checkbox",
                            value: "south",
                            disabled: function(key, opt){
                                return !opt.inputs.wall_south.$input.prop("checked");
                            }
                        },
                        laser_east: {
                            name: "East",
                            type: "checkbox",
                            value: "east",
                            disabled: function(key, opt){
                                return !opt.inputs.wall_east.$input.prop("checked");
                            }
                        },
                        laser_west: {
                            name: "West",
                            type: "checkbox",
                            value: "west",
                            disabled: function(key, opt){
                                return !opt.inputs.wall_west.$input.prop("checked");
                            }
                        }
                    }
                },
            }, 
            events: {
                show: function(opt) {
                    var $this = this;
                    var dirs = ["north", "south", "west", "east"];
                    var data = {};
                    Object.values(dirs).forEach((dir) => {
                        data["wall_" + dir] = $this.hasClass("tile_wall_" + dir);
                        data["laser_" + dir] = $this.children(".laser").hasClass("tile_laser_" + dir);
                        if ($this.find(".orientation").hasClass("tile_orientation_" + dir)) {
                            data["orientation"] = dir;
                        }
                    });
                    if ($this.hasClass("tile_blank")) {
                        data["tile_type"] = "blank";
                    } else if ($this.hasClass("tile_conveyour")) {
                        data["tile_type"] = "conveyour";   
                    } else if ($this.hasClass("tile_pusher")) {
                        data["tile_type"] = "pusher";
                    }
                    $.contextMenu.setInputValues(opt, data);
                }, 
                hide: function(opt) {
                    var $this = this;
                    var dirs = ["north", "south", "west", "east"];
                    var data = {};
                    $.contextMenu.getInputValues(opt, data);
                    Object.values(dirs).forEach((dir) => {
                        $this.removeClass("tile_wall_" + dir);
                        if (data["wall_" + dir])
                            $this.addClass("tile_wall_" + dir);
                        $this.children(".laser").removeClass("tile_laser_" + dir);
                        if (data["laser_" + dir])
                            $this.children(".laser").addClass("tile_laser_" + dir);
                        $this.find(".orientation").removeClass("tile_orientation_" + dir);
                        if (data["orientation"] == dir)
                            $this.find(".orientation").addClass("tile_orientation_" + dir);
                    });

                    $this.removeClass("tile_blank");
                    $this.removeClass("tile_conveyour");
                    $this.removeClass("tile_pusher");
                    if (data["tile_type"] == "blank") {
                        $this.addClass("tile_blank");
                    } else if (data["tile_type"] == "conveyour") {
                        $this.addClass("tile_conveyour");
                    } else if (data["tile_type"] == "pusher") {
                        $this.addClass("tile_pusher");
                    }

                }
            }
        });

        Object.values(map.map_data).forEach((tile) => {
            var cell = map_layout[tile.x][tile.y];
            cell.addClass('tile_' + tile.tile);

            if (tile.orientation != undefined)
                cell.find(".orientation").addClass("tile_orientation_" + tile.orientation);

            if (tile.tile_add_ons != undefined) {
                Object.values(tile.tile_add_ons).forEach((add_on) => {
                    switch(add_on.name) {
                        case "wall":
                            cell.addClass('tile_wall_' + add_on.edge);
                            break;
                        case "laser":
                            cell.find(".laser").addClass('tile_laser_' + add_on.edge);
                            break;
                    }
                })
            }
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
        SetupMapSpace(maps[this.value], this.value);
    });
    $(".tools").append(mapper);
});