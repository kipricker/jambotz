import crypto from 'crypto';

import Deck from './Deck';
import Player from './Player';

export default class Game {
    config = {
        playerCount: 2,
        handSize: 5,
    };

    id = '';
    playersLUT = {};
    players = [];
    active = false;
    actions = [
        { move: 1},
        { move: 1},
        { move: 1},
        { move: 1},
    ];

    deck = new Deck();
    
    constructor() {
        this.id = crypto.randomBytes(16).toString('hex');
    }

    join(player) {
        this.players.push(player);
        this.playersLUT[player.id] = player;
        if (this.players.length >= this.config.playerCount) {
            this.startGame();
        }
    }

    startGame() {
        this.active = true;
        this.deck.shuffle();
        this.deal();
    }

    deal() {
        const p1 = this.players[0];
        const p2 = this.players[1];
        const handSize = this.config.handSize;
        while (p1.hand.length < handSize || p2.hand.length < handSize) {
            if (p1.hand.length < handSize) {
                const card = this.deck.pullCard();
                p1.hand.push(card);
            }

            if (p2.hand.length < handSize) {
                const card = this.deck.pullCard();
                p2.hand.push(card);                    
            }
        }
    }

    getPlayerHand(playerID) {
        return this.playersLUT[playerID].hand;
    }

    getActionsSince(lastSeen) {
        if (this.actions.length > 0) {
            return this.actions.slice(lastSeen + 1);
        } else {
            return [];
        }
    }
}
