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

export default router;