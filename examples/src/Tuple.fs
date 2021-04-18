module Tuple

let mapFirst f (x, y) = (f x, y)

let mapSecond f (x, y) = (x, f y)
