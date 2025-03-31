using WebXeDap.Models;

namespace WebXeDap.Repositories
{
    public class EFDiaChiNhanHangRepository
    {
        private List<DiaChiNhanHang> _dsDiaChi = new List<DiaChiNhanHang>();

        public List<DiaChiNhanHang> GetAll() => _dsDiaChi;

        public DiaChiNhanHang GetById(int id) => _dsDiaChi.FirstOrDefault(d => d.Id == id);

        public void Add(DiaChiNhanHang diaChi)
        {
            diaChi.Id = _dsDiaChi.Count + 1;
            _dsDiaChi.Add(diaChi);
        }

        public void Update(DiaChiNhanHang diaChi)
        {
            var existing = GetById(diaChi.Id);
            if (existing != null)
            {
                existing.TenNguoiNhan = diaChi.TenNguoiNhan;
                existing.SoDienThoai = diaChi.SoDienThoai;
                existing.DiaChi = diaChi.DiaChi;
                existing.TinhThanh = diaChi.TinhThanh;
                existing.QuanHuyen = diaChi.QuanHuyen;
                existing.PhuongXa = diaChi.PhuongXa;
            }
        }

        public void Delete(int id)
        {
            var diaChi = GetById(id);
            if (diaChi != null) _dsDiaChi.Remove(diaChi);
        }
    }
}
