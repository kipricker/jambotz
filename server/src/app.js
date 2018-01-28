require("babel-core/register");
require("babel-polyfill");

import express from 'express';
import lobby from './routes/lobby';
import game from './routes/game';

const app = express();
const port = process.env.PORT || 3000;


app.all((req, res, next) => {
    next();
});

app.get('/', (req, res) => {
              
});

app.use('/lobby', lobby);
app.use('/game', game);

app.listen(port, () => console.log(`Listening on port ${port}!`));
