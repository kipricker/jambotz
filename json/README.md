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