import collections.abc

class CaloriePack(collections.abc.MutableSequence):
    def __init__(self, calories = None):
        self.pack = []
        if calories is not None:
            if isinstance(calories, collections.abc.Iterable):
                self.extend(calories)
            else:
                self.append(calories)

    def __getitem__(self, idx):
        return self.pack.__getitem__(idx)

    def __len__(self):
        return len(self.pack)

    def __setitem__(self, idx, value):
        if not isinstance(value, int):
            raise ValueError("values must be ints")
        self.pack.__setitem__(idx, value)

    def __delitem__(self, idx):
        self.pack.__delitem__(idx)

    def insert(self, idx, value):
        if not isinstance(value, int):
            raise ValueError("values must be ints")
        self.pack.insert(idx, value)

    def __eq__(self, other):
        if not isinstance(other, self.__class__):
            return NotImplemented
        return self.caloriecount == other.caloriecount

    def __ne__(self, other):
        if not isinstance(other, self.__class__):
            return NotImplemented
        return self.caloriecount == other.caloriecount

    def __lt__(self, other):
        if not isinstance(other, self.__class__):
            return NotImplemented
        return self.caloriecount < other.caloriecount

    def __le__(self, other):
        if not isinstance(other, self.__class__):
            return NotImplemented
        return self.caloriecount <= other.caloriecount

    def __gt__(self, other):
        if not isinstance(other, self.__class__):
            return NotImplemented
        return self.caloriecount > other.caloriecount

    def __ge__(self, other):
        if not isinstance(other, self.__class__):
            return NotImplemented
        return self.caloriecount >= other.caloriecount

    @property
    def caloriecount(self):
        return sum(self.pack)

class Elf:
    def __init__(self, calories = None):
        self.pack = CaloriePack(calories)

    def load(self, fd, failIfEOF=False):
        line = fd.readline()
        if not line and failIfEOF:
            raise EOFError()
        while line and line != fd.newlines:
            calories = int(line.strip(fd.newlines))
            self.pack.append(calories)
            line = fd.readline()


if __name__ == '__main__':
    import argparse
    ap = argparse.ArgumentParser()
    ap.add_argument('infile', type=argparse.FileType('r'))
    args = ap.parse_args()
    
    elves = []
    while True:
        try:
            elf = Elf()
            elf.load(args.infile, failIfEOF=True)
            elves.append(elf)
        except EOFError:
            break

    elves.sort(key = lambda elf:elf.pack, reverse=True)

    print(f'''Highest calorie count: {elves[0].pack.caloriecount}''')
    print(f'''Calorie count for top three elves: {sum(elf.pack.caloriecount for elf in elves[0:3])}''')
