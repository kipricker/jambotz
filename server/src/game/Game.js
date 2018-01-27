import crypto from 'crypto';

import Player from './Player';

export default class Game {
    config = {
        playerCount: 2,
    };

    id = '';
    players = [];
    active = false;
    states = [];

    constructor() {
        this.id = crypto.randomBytes(16).toString('hex');
    }

    join(player) {
        this.players.push(player);
        if (this.players.length >= this.config.playerCount) {
            this.active = true;
        }
    }

    getStateSince(lastSeen) {
        if (this.states.length > 0) {
            return this.states.slice(lastSeen + 1);
        } else {
            return [];
        }
    }
}
