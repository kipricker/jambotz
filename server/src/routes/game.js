import express from 'express';
import Game from '../game/Game';

const router = express.Router();

global.activeGames = {};

router.post('/heartbeat', express.urlencoded(), (req, res, next) => {
    if (global.activeGames[req.body.gameID]) {
        const latestActions = global.activeGames[req.body.gameID].getActionsSince(req.body.lastSeen);
        res.status(200).json(latestActions);
    } else {
        res.status(200).json([]);
    }

});

export default router;