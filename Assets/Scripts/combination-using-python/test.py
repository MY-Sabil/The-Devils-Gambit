import imp
from itertools import permutations
import json

nums = []
i = 0
while i < 1000:
    s = (f"{i:03}")
    nums.append(s)
    i += 1

with open("nums.json", "w") as f:
    json.dump(nums, f)


op = ["+", "-", "*", "/"]
perm = permutations(op, 2) # I had to add duplicate permutations like ["+", "+"] or ["-", "-"] myself in the json file

with open("op.json", "w") as fi:
    json.dump(perm, fi)