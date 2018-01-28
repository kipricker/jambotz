import Game from './Game';

export default class Lobby {
    pendingGames = [];

    createGame(player) {

    }

    findGame(player) {
        let game;
        if (this.pendingGames.length > 0) {
            game = this.pendingGames[0];
            
            const now = Math.floor( Date.now() / 1000 );
            if (now - game.createdOn > 60) {
                this.pendingGames = this.pendingGames.slice(1);
                game = null;
            }
        }

        if (!game) {
            game = new Game();
            this.pendingGames.push(game);
            global.games[game.id] = game;
        }

        game.join(player);
        if (game.active) {
            this.pendingGames = this.pendingGames.slice(1);
        }
        return game;
    }
}
