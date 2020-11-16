using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace SobelAlgImage.Helper
{
    public class ParallelProcessHelper
    {
        public Bitmap AlgorithmChooser(int algorithmChooser, Bitmap bitmap, int greyScale)
        {
            SobelAlgorithm imageProcessAlg = new SobelAlgorithm();

            return algorithmChooser == 1 ? imageProcessAlg.SobelFilter(bitmap, greyScale) : imageProcessAlg.ConvolutionFilter(bitmap);
        }

        public List<Bitmap> ConverBitmapsWithTasks(IEnumerable<Bitmap> collectedBitmaps, int tiles, int algorithmChooser, int greyScale)
        {
            var tasks = new List<Task>();
            List<Bitmap> resultedListOfBitmaps = new Bitmap[tiles].ToList();

            foreach (var i in Enumerable.Range(0, tiles))
                tasks.Add(new Task(() =>
                    resultedListOfBitmaps[i] = AlgorithmChooser(algorithmChooser, collectedBitmaps.ToList()[i], greyScale)));

            foreach (var t in tasks)
                t.Start();

            Task.WaitAll(tasks.ToArray());

            return resultedListOfBitmaps;
        }

        public List<Bitmap> ConverBitmapsWithSheduler(IEnumerable<Bitmap> collectedBitmaps, int tiles, int algorithmChooser, int greyScale, int splitNumber)
        {
            List<Bitmap> resultedListOfBitmaps = new Bitmap[splitNumber].ToList();
            var numberList = Enumerable.Range(0, splitNumber).ToList();

            var schedulerPair = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, maxConcurrencyLevel: tiles);
            TaskScheduler scheduler = schedulerPair.ConcurrentScheduler;
            ParallelOptions options = new ParallelOptions
            {
                TaskScheduler = scheduler
            };

            Parallel.ForEach(numberList, options, taskInt => resultedListOfBitmaps[taskInt] = AlgorithmChooser(algorithmChooser, collectedBitmaps.ToList()[taskInt], greyScale));

            return resultedListOfBitmaps;
        }
    }
}
