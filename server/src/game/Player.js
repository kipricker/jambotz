import crypto from 'crypto';

export default class Player {
    id = '';
    hand = [];

    constructor() {
        this.id = crypto.randomBytes(16).toString('hex');
    }

}
