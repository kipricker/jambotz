import crypto from 'crypto';

export default class Player {
    id = '';

    constructor() {
        this.id = crypto.randomBytes(16).toString('hex');
    }

}
