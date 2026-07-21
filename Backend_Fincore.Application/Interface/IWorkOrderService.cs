using Backend_Fincore.Application.DTOs.WorkOrder;

namespace Backend_Fincore.Interface
{
    public interface IWorkOrderService
    {
        Task<List<WorkOrderReadDTO>> GetAll();

        Task<WorkOrderReadDTO?> GetById(int id);

        Task<WorkOrderReadDTO> Create(WorkOrderWriteDTO dto);

        Task<WorkOrderReadDTO?> Update(int id, WorkOrderWriteDTO dto);

        Task<bool> Delete(int id);
    }
}
