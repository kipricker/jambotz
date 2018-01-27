import Game from './Game';

export default class Lobby {
    pendingGames = [];

    createGame(player) {

    }

    findGame(player) {
        let game;
        if (this.pendingGames.length > 0) {
            game = this.pendingGames[0];
        } else {
            game = new Game();
            this.pendingGames.push(game);
        }
        game.join(player);
        if (game.active) {
            this.pendingGames = this.pendingGames.slice(1);
            global.activeGames[game.id] = game;
        }
        return game;
    }
}
