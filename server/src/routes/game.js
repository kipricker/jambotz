import express from 'express';
import Game from '../game/Game';

const router = express.Router();

global.activeGames = {};

router.post('/heartbeat', express.urlencoded(), (req, res, next) => {
    console.log(req.body)
    res.status(200).json({
        // global.activeGames[]
    });
});

export default router;