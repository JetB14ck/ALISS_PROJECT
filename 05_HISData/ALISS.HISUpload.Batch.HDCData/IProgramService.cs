using System.Threading.Tasks;

namespace ALISS.HISUpload.Batch.HDCData
{
    public interface IProgramService
    {
        Task BATCH_HDCData_RUN();
    }
}