import crypto from 'crypto';

import Player from './Player';

export default class Game {
    config = {
        playerCount: 2,
    };

    id = '';
    players = [];
    active = false;
    actions = [
        { move: 1},
        { move: 1},
        { move: 1},
        { move: 1},
    ];

    constructor() {
        this.id = crypto.randomBytes(16).toString('hex');
    }

    join(player) {
        this.players.push(player);
        if (this.players.length >= this.config.playerCount) {
            this.active = true;
        }
    }

    getActionsSince(lastSeen) {
        if (this.actions.length > 0) {
            return this.actions.slice(lastSeen + 1);
        } else {
            return [];
        }
    }
}
