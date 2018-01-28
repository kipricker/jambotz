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
    actions = [];

    deck = new Deck();

    turnCards = {};
    turnHands = [];
    
    constructor() {
        this.id = crypto.randomBytes(16).toString('hex');
    }

    join(player) {
        player.number = this.players.length;
        this.players.push(player);
        this.playersLUT[player.id] = player;
        if (this.players.length >= this.config.playerCount) {
            this.startGame();
        }
    }

    startGame() {
        this.active = true;
        this.actions.push({ gameStarted: true });

        this.deck.shuffle();
        this.deal();
    }

    deal() {
        const handSize = this.config.handSize;
        for (let i = 0; i < handSize; i++) {
            this.players.forEach((player) => {
                const card = this.deck.pullCard();
                player.dealCard(card);
            });
        }
        this.actions.push({ handsDealt: true });
    }

    playHand(playerID, hand) {
        if (hand.length === 5) {
            const player = this.playersLUT[playerID];
            const action = {}; 
            this.turnHands.push({
                player_played_hand: { playerNumber: player.number, hand }, 
            });
        }            
        if (this.turnHands.length === this.config.playerCount) {
            this.players.forEach((player) => {
                const hand = player.discardHand();
                this.deck.discardCards(hand);
            });

            this.actions = this.actions.concat(this.turnHands);
            this.turnHands = [];

            this.deal();
        }
    }

    sendCards(playerID, cards) {
        if (cards.length === 2) {
            const player = this.playersLUT[playerID];
            const action = {};
            this.turnCards[player.number] = cards;
        }
        if (Object.keys(this.turnCards).length === this.config.playerCount) {
            for (let i = 0; i < this.players.length; i++) {
                const p1 = this.players[i];
                const p2 = i + 1 < this.players.length ? this.players[i + 1] : this.players[0];
                const p1Cards = p1.removeCards(this.turnCards[i]);
                p2.addCards(p1Cards);
            }
            this.actions.push({ player_cards_sent: true });
            this.turnCards = {};
        }
    }

    getHand(playerID) {
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
