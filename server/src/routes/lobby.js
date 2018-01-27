import express from 'express';
import Lobby from '../game/Lobby';
import Player from '../game/Player';

const router = express.Router();
const lobby = new Lobby();

router.get('/', (req, res, next) => {

});

router.post('/', express.urlencoded(), (req, res, next) => {
    console.log(req.body)
    res.send('Welcome to the lobby')
});

router.post('/create', express.urlencoded(), (req, res, next) => {
    // lobby.createGame();
});

router.post('/join', express.urlencoded(), (req, res, next) => {
    const player = new Player();
    const game = lobby.findGame(player);

    res.status(200).json({
        playerID: player.id,
        gameID: game.id,
    });
});

export default router;