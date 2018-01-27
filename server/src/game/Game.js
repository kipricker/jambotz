export default class Game {
    config = {
        playerCount = 2,
    };

    players = [];
    active = false;

    constructor() {}

    join(player) {
        players.push(player);
        if (players.length === config.playerCount) {
            active = true;
        }
    }
}
