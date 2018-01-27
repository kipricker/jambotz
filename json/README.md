Actions:
**Example
    "dummy" : {
        # small icon to show card action
        "icon" : "something.png",
        # smaller icon at the bottom right of the main icon
        # show possible added action to normal action
        "sub_icon" : "something_else.png",
        # obligitory enabled/disabled flag, so we don't have to delete actions
        "enabled" : false,
        # multiplier of base priority, so "sharpshooter would become default
        # faster than a move
        "priority_modifier" : 1.1,
        # three outcome-sets of this action
        "on_hit" : {
            # health modifier of target (that got hit)
            "target_modifier" : -1,  # lost HP
            # health of player who did this action
            "player_modifier" : 1,  # gained HP
            # positive movement is forward, negative is back
            "target_move" : 1,
            "player_move" : 1,
            # positive is clockwise, negative is counter-clockwise
            "turn" : 1,
            "look" : 1,
            # chained actions, check for recursive loops...
            "actions" : []
        },
        "on_miss" : {},
        "passive" : {}
    },

Cards:
**Example
"sux_4_u" : {
    # not stored in definitions, assigned by server, 1-10
    "priority" : 9,
    "name" : "You Die!",
    "flavor" : "Sucks to be you!",
    # list of actions in order
    "actions" : ["fire", "fire", "fire", "fire"]
}

Maps:
**Example snippet
{
    "height" : 5,
    "width" : 5,
    # Multiple start locations, can be randomly chosen by server for players
    "starting_locations" : [
        [0,0],
        [4,4]
    ],
    # unordered list of tiles and usage
    "map_data" : [
        {
            # row, column
            "location" : [0,0],
            "tile" : "blank_tile",
            # N S E W
            "walls" : [0,0,1,1],
            # which wall has the special
            "special" : "bumper",
            "special_wall" : "north",
            # for things like conveyours, horizontal or vertical
            "orientation" : "vertical",
            # tiles have variations, this should match one in the tile's list
            "variation" : "slow",
        }
}