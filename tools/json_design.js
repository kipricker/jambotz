$(function() {
    var cards_json = "../json/cards.json";
    var actions_json = "../json/actions.json";
    var tiles_json = "../json/tiles.json";
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
});