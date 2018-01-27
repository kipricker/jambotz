$(function() {
    var cards_json = "../json/cards.json";
    var actions_json = "../json/actions.json";
    var cards;
    var actions;
    $.getJSON(cards_json, function(json) {
        cards = JSON.parse(json);
    });
    $.getJSON(actions_json, function(json) {
        actions = JSON.parse(json);
    });
});