from PIL import Image

import numpy as np

import sys

print('Num args: ', len(sys.argv))
print('Arg list: ', str(sys.argv))


if len(sys.argv) < 11 or '-h' in sys.argv:
    print('Usage: python DecomposeImage ["png file"] <r1> <g1> <b1> <r2> <g2> <b2> <r3> <g3> <b3>\nSpecify RGB values in 0-255')

filename = sys.argv[1]

colors = np.array([[sys.argv[2], sys.argv[3], sys.argv[4]], 
                   [sys.argv[5], sys.argv[6], sys.argv[7]], 
                   [sys.argv[8], sys.argv[9], sys.argv[10]]])
colors = np.transpose(colors)
colors = np.divide(colors.astype(float), 255.0)

def solve_lstsq(texel):
    return np.append(np.linalg.lstsq(colors, texel, rcond=None)[0], [1.0])

with Image.open(filename) as im:
    data = np.asarray(im)
    print("Array dimensions: %s"%(repr(data.shape)))
    data = data.astype(float)  # convert to floats
    data = np.divide(data, 255.0)  # normalize
    data = np.delete(data, 3, 2)  # remove alpha channel
    rows = data.shape[0]
    columns = data.shape[1]
    print(str(rows) + ", " + str(columns))
    data = data.reshape(-1, 3)
#    print(data)
    print("Array dimensions: %s"%(repr(data.shape)))
    data = np.apply_along_axis(solve_lstsq, 1, data)
    print(data)
    print(np.apply_along_axis(min, 0, data))
    print(np.apply_along_axis(max, 0, data))
    data = data.reshape(rows, columns, 4)

    newIm = Image.fromarray(data)
    newIm.show()
    


    # print(data[1])
    # testData = np.linalg.lstsq(colors, data[1], rcond=None)[0]
    # print(testData)

    # for i in range(0,data.shape[0]):
    #     data[i] = np.linalg.lstsq(colors, data[i], rcond=None)
#    print(data)
    # for row in data:
    #     for texel in row:
    #         texel = np.linalg.lstsq(colors, texel, rcond=None)
            #print(texel)
    #print(np.max(data))

