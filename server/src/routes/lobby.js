import express from 'express';

const router = express.Router();


router.get('/', (req, res, next) => {
    console.log(req.body)
    res.send('Welcome to the lobby')
});


router.post('/', express.urlencoded(), (req, res, next) => {
    console.log(req.body)
    res.send('Welcome to the lobby')
});

export default router;