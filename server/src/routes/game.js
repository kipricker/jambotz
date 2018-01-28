import express from 'express';
import Game from '../game/Game';

const router = express.Router();

global.games = {};

router.post('/heartbeat', express.urlencoded(), (req, res, next) => {
    let latestActions = [];
    let active = false;
    let game = global.games[req.body.gameID];
    if (game) {
        active = game.active;
        latestActions = game.getActionsSince(req.body.lastSeen);
    }
    res.status(200).json({ exists: !!game, active, latestActions });
});


router.post('/send-cards', express.urlencoded(), (req, res, next) => {
    let game = global.games[req.body.gameID];    
    if (game) {
        game.sendCards(req.body.playerID, JSON.parse(req.body.cards));
    }
    res.sendStatus(200);
});

router.post('/play-hand', express.urlencoded(), (req, res, next) => {
    let game = global.games[req.body.gameID];
    if (game) {
        game.playHand(req.body.playerID, JSON.parse(req.body.hand));
    }
    res.sendStatus(200);
});

router.post('/get-hand', express.urlencoded(), (req, res, next) => {
    let game = global.games[req.body.gameID];
    let hand = [];
    if (game) {
        hand = game.getHand(req.body.playerID);
    }
    res.status(200).json({ hand });
});


export default router;