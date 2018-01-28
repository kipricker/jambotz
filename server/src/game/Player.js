import crypto from 'crypto';

export default class Player {
    id = '';
    handLUT = {};

    get hand() {
        return Object.values(this.handLUT);
    }

    constructor() {
        this.id = crypto.randomBytes(16).toString('hex');
    }

    addCards(cards) {
        cards.forEach((card) => {
            this.handLUT[card.id] = card;
        });
    }

    removeCards(cards) {
        const removed = [];
        cards.forEach((card) => {
            removed.push(this.handLUT[card.id]);
            delete this.handLUT[card.id];
        });
        return removed;
    }

    dealCard(card) {
        this.handLUT[card.id] = card;
    }

    discardHand() {
        const hand = this.hand;
        this.handLUT = {};
        return hand;
    }
}
