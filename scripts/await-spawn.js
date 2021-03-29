const { spawn } = require('child_process')

module.exports = (...args) => {
    const child = spawn(...args)

    const promise = new Promise((resolve, reject) => {
        child.on('error', reject)

        child.on('exit', code => {
            if (code === 0) {
                resolve()
            } else {
                const err = new Error(`child exited with code ${code}`)
                err.code = code
                reject(err)
            }
        })
    })

    promise.child = child

    return promise
}
