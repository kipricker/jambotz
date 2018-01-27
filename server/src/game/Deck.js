import randomNumber from 'random-number-csprng';

import Card from './Card';

function randomInt (low, high) {
    return Math.floor(Math.random() * (high - low) + low);
}

export default class Deck {
    cards = [
        { name: 'a' },
        { name: 'b' },
        { name: 'c' },
        { name: 'd' },
        { name: 'e' },
        { name: 'f' },
        { name: 'g' },
        { name: 'h' },
        { name: 'i' },
        { name: 'j' },
        { name: 'k' },
    ];
    discarded = [];

    shuffle() {        
        const shuffled = [];
        while (this.cards.length > 1) {
            const cardIndex = randomInt(0, this.cards.length);
            shuffled.push(this.cards.splice(cardIndex, 1)[0]);
        }

        if (this.cards.length > 0) {
            shuffled.push(this.cards.splice(0, 1)[0]);
        }

        this.cards = shuffled;
    }

    shuffleInDiscarded() {
        this.cards = this.discarded;
        this.shuffle();
    }

    pullCard() {
        if (this.cards.length === 0) {
            this.shuffleInDiscarded();
        }

        const card = this.cards[0];
        this.cards = this.cards.splice(1);
        return card;
    }

    discardCard(card) {
        this.discarded.push(card);
    }
}
