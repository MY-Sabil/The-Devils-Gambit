import json, operator

nums = []
op = []
ops = {"+" : operator.add, "-": operator.sub, "*": operator.mul, "/": operator.truediv}

with open("nums.json", "r") as f:
    nums = json.load(f)

with open("op.json", "r") as fi:
    op = json.load(fi)

result = {}

for i in range(0, 98):
    result[i] = []

for j in range(0, 998):
    for k in range(0, 15):
        try:
            if(op[k][0] == "+"):
                a = int(nums[j][0]) + int(nums[j][1])
            elif(op[k][0] == "-"):
                a = int(nums[j][0]) - int(nums[j][1])
            elif(op[k][0] == "*"):
                a = int(nums[j][0]) * int(nums[j][1])
            elif(op[k][0] == "/"):
                a = int(nums[j][0]) / int(nums[j][1])

            if(op[k][1] == "+"):
                b = a + int(nums[j][2])
            elif(op[k][1] == "+"):
                b = a - int(nums[j][2])
            elif(op[k][1] == "+"):
                b = a * int(nums[j][2])
            elif(op[k][1] == "+"):
                b = a / int(nums[j][2])
        except ZeroDivisionError:
            b = 0

        if b < 99 and b > 0 and b == int(b):
            li = [int(nums[j][0]), int(nums[j][1]), int(nums[j][2])]
            result[b].append(li)

with open("result.json", "w") as file:
    json.dump(result, file)

