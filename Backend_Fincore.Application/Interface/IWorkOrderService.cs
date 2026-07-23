using Backend_Fincore.Application.DTOs.WorkOrder;

namespace Backend_Fincore.Interface
{
    public interface IWorkOrderService
    {
      
            Task<WorkOrderReadDTO> Create(WorkOrderWriteDTO dto);

            Task<List<WorkOrderReadDTO>> GetAll(int userId);

            Task<WorkOrderReadDTO> Update(
                int workOrderId,
                WorkOrderWriteDTO dto);

            Task<bool> Delete(int workOrderId);

            Task<WorkOrderReadDTO> Verify(
                int workOrderId,
                int approvedBy,
                WorkOrderVerifyDTO dto);
        

    }
}
