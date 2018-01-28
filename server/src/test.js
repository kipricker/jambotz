import axios from 'axios';
import qs from 'qs';

axios.defaults.baseURL = 'http://localhost:3000';
axios.defaults.headers.post['Content-Type'] = 'application/x-www-form-urlencoded';

let params = qs.stringify({  
    
});

let p1;
let p2;

let joinPromises = [];

joinPromises.push(axios.post('/lobby/join', params));
joinPromises.push(axios.post('/lobby/join', params));

Promise.all(joinPromises).then((responses) => {
    let getHandPromises = [];
    let p1 = responses[0].data;
    let p2 = responses[1].data;

    let p1Params = qs.stringify(p1);
    let p2Params = qs.stringify(p2);
    getHandPromises.push(axios.post('/game/get-hand', p1Params));
    getHandPromises.push(axios.post('/game/get-hand', p2Params));
    
    Promise.all(getHandPromises).then((getHandResponses) => {
        // getHandResponses[0].data.hand.forEach((card) => {console.log(card.id)})
        let sendCardsPromises = [];
        p1Params = qs.stringify(Object.assign({}, { cards: JSON.stringify(getHandResponses[0].data.hand.slice(0, 2)) }, p1));
        p2Params = qs.stringify(Object.assign({}, { cards: JSON.stringify(getHandResponses[1].data.hand.slice(0, 2)) }, p2));
        sendCardsPromises.push(axios.post('/game/send-cards', p1Params));
        sendCardsPromises.push(axios.post('/game/send-cards', p2Params));

        Promise.all(sendCardsPromises).then((responses) => {
            let heartbeatPromises = [];
            p1Params = qs.stringify(p1);
            p2Params = qs.stringify(p1);
            heartbeatPromises.push(axios.post('/game/get-hand', p1Params));
            heartbeatPromises.push(axios.post('/game/get-hand', p2Params));
            Promise.all(heartbeatPromises).then((heartbeatResponses) => {
                // heartbeatResponses[0].data.hand.forEach((card) => {console.log(card.id)})
                let playHandPromises = [];
                p1Params = qs.stringify(Object.assign({}, { hand: JSON.stringify(heartbeatResponses[0].data.hand) }, p1));
                p2Params = qs.stringify(Object.assign({}, { hand: JSON.stringify(heartbeatResponses[1].data.hand) }, p2));
                playHandPromises.push(axios.post('/game/play-hand', p1Params));
                playHandPromises.push(axios.post('/game/play-hand', p2Params));

                Promise.all(playHandPromises).then((playHandResponses) => {
                    let heartbeat2Promises = [];
                    p1Params = qs.stringify(Object.assign({ lastSeen: -1 }, p1));
                    p2Params = qs.stringify(Object.assign({ lastSeen: -1 }, p2));
                    heartbeat2Promises.push(axios.post('/game/heartbeat', p1Params));
                    heartbeat2Promises.push(axios.post('/game/heartbeat', p2Params));

                    Promise.all(heartbeat2Promises).then((heartbeat2Responses) => {
                        console.log(heartbeat2Responses[0].data);
                        console.log(heartbeat2Responses[1].data);
                    });
                });

            });
        });
    });
})