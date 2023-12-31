import http from 'k6/http';

const url = 'http://localhost:1030';

export const options = {
    thresholds: {
        http_req_failed: ['rate < 0.01'],
        http_req_duration: ['p(95) < 100'],
    },
    scenarios: {
      load: {
        executor: 'constant-arrival-rate',
        duration: '5m', // total duration
        preAllocatedVUs: 250, // to allocate runtime resources
        rate: 1500, // number of constant iterations given `timeUnit`
        timeUnit: '10s',
      },
      stress: {
        executor: "ramping-arrival-rate",
        preAllocatedVUs: 250,
        timeUnit: "10s",
        startRate: 50,
        stages: [
          { duration: "1m", target: 25 }, // below normal load
          { duration: "2m", target: 350 },
          { duration: "2m", target: 750 }, // normal load
          { duration: "2m", target: 1500 },
          { duration: "2m", target: 2000 }, // around the breaking point
          { duration: "2m", target: 450 },
          { duration: "2m", target: 250 }, // beyond the breaking point
          { duration: "1m", target: 0 }, // scale down. Recovery stage.
        ],
      },
      soak: {
        stages: [
            { duration: '2m', target: 200 },
            { duration: '56m', target: 200 },
            { duration: '2m', target: 0 },
          ],
      }
    },
};

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'
    .replace(/[xy]/g, function (c) {
        const r = Math.random() * 16 | 0, 
            v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

export default function () {
    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    var userId = uuidv4();
    http.post(`${url}/connect/${userId}`, null, params, {
        tags: { name: 'connect' },
    });
}