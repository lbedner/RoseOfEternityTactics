import sys
from PIL import Image

from pygame import image

sizes = [2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192]  # po2 sizes

def get_closest(y):
    """ Return the closest power of 2 in either direction"""
    return min(sizes, key=lambda x: abs(x - y))


def po2(im, threshold=0.25):
    """ 
    Return a resized image that is a power of 2 
    (Checking that if image size is reduced it was fairly close to that size anyway based on threshold)
    """
    width, height = im.size
    largest_dim = max(width, height)
    new_dim = max(get_closest(width), get_closest(height))  # new dimension will be largest of x or y po2

    if new_dim < largest_dim:  # if it's smaller, make sure its within threshold
        if (largest_dim - new_dim) > int(new_dim * threshold):
            new_dim = sizes[sizes.index(new_dim) + 1]
            return im.resize((new_dim, new_dim), resample=Image.BICUBIC)
        else:
            return im.resize((new_dim, new_dim), resample=Image.LANCZOS)
    else:
        return im.resize((new_dim, new_dim), resample=Image.BICUBIC)

def save_targa(im, arg):
    """ Convert PIL image to pygame image and save. PIL cannot save tga and no other library works with PIL """
    image.save(image.fromstring(im.tobytes(), im.size, im.mode), arg.split('.', 1)[0] + '.tga')

def main():
    threshold: float = 0.25
    compression: int = 9
    try:
        # warnings.simplefilter('ignore', Image.DecompressionBombWarning)
        if len(sys.argv) > 1:
            for arg in sys.argv[1:]:
                try:
                    print(f"Opening {arg}...")
                    im = Image.open(arg)
                    if im.format.upper() == "TGA":
                        save_targa(po2(im, threshold), arg)
                    else:
                        po2(im, threshold).save(arg, im.format.lower(), compress_level=compression)
                except IOError:
                    print("IO ERROR: Is file an image? -> ", arg)
    except MemoryError:
        print("OOM")

if __name__ == '__main__':
    main()