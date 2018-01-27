$(function() {
    var cards_json = "../client/Assets/Resources/json/cards.json";
    var actions_json = "../client/Assets/Resources/json/actions.json";
    var tiles_json = "../client/Assets/Resources/json/tiles.json";
    var cards;
    var actions;
    var tiles;
    $.getJSON(cards_json, function(json) {
       cards = json;
    });
    $.getJSON(actions_json, function(json) {
       actions = json;
    });
    $.getJSON(tiles_json, function(json) {
       tiles = json;
    });

    var cards_button = "<button class='tool'>Cards</button>";
    cards_button = $(cards_button).click(function() {
        $(".tool_workspace").empty().append("HI");
    });
    var actions_button = "<button class='tool'>Actions</button>";
    var tiles_button = "<button class='tool'>Tiles</button>";

    $(".tool_list").append(cards_button);
    $(".tool_list").append(actions_button);
    $(".tool_list").append(tiles_button);
});