
public interface IAttackModule
{
    void Init(PlayerBehavior owner);
    void Tick();                 // nếu cần xử lý input/aim mỗi frame
    bool TryAttack();            // trả true nếu đánh được
    
}

