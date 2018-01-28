require("babel-core/register");
require("babel-polyfill");

import axios from 'axios';
import qs from 'qs';

axios.defaults.baseURL = 'http://localhost:3000';
// axios.defaults.baseURL = 'https://dry-spire-78198.herokuapp.com/';
axios.defaults.headers.post['Content-Type'] = 'application/x-www-form-urlencoded';

let lastSeen = 0;
let p1;
let active = false;
let currentHand = [];

const joinRoom = async () => {
    const params = qs.stringify({});
    let response = await axios.post('/lobby/join', params);
    p1 = response.data;
    console.log('joined', p1)
}

const getHand = async () => {
    console.log('getting hand');
    let params = qs.stringify(p1);
    let response = await axios.post('/game/get-hand', params);
    currentHand = response.data.hand;
    console.log(currentHand.map(card => card.name));
}

const sendCards = async () => {
    console.log('sending cards', currentHand.slice(0, 2).map(card => card.name));
    let params = qs.stringify(Object.assign({}, { cards: JSON.stringify(currentHand.slice(0, 2)) }, p1));
    await axios.post('/game/send-cards', params);
}

const playHand = async () => {
    console.log('playing hand');
    let params = qs.stringify(Object.assign({}, { hand: JSON.stringify(currentHand) }, p1));
    await axios.post('/game/play-hand', params);
}

const heartbeat = async () => {
    const params = qs.stringify(Object.assign({ lastSeen }, p1));
    let response = await axios.post('/game/heartbeat', params);
    return response.data;
}

let stage = 0;

const run = async () => {
    setInterval(async () => {
        if (!p1) {
            await joinRoom();
        }

        const hb = await heartbeat();

        lastSeen += hb.latestActions.length;
        hb.latestActions.forEach(async (action) => {
            if (action.gameStarted) {
                active = true;
            }

            if (action.handsDealt) {
                await getHand();
                if (stage === 0) {
                    sendCards();
                    stage++;
                }
            }

            if (action.player_cards_sent) {
                await getHand();
                await playHand();
                stage = 0;
            }

            if (action.player_played_hand) {
                console.log('beep boop beep');
            }
        });

        if (hb.ended) {
            p1 = null;
            console.log('game ended');
        }
    }, 1000);
}

run();
