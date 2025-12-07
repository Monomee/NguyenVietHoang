# sonat_intern_test
Dưới đây là quá trình làm với suy nghĩ của em, em vừa làm cũng có tìm hiểu thêm cả trên mạng và trò chơi này em cũng đã từng chơi rồi, em nêu cả cách triển khai nên có thể hơi dài, em cũng đánh số để có thể dễ theo dõi hơn. Em cảm ơn nhiều ạ  
(Tóm tắt ngắn gọn em đã làm những gì: 
- Có cơ chế chơi theo kiểu Water Sort Puzzle
- Sử dụng các design pattern: Singleton, Command, State, Observer và dùng Scriptable Object
- Có thể gen màn theo độ khó
- có dotween nhưng chỉ đơn giản ở phần nắp đóng)

# Mục tiêu 
Water sort puzzle game

# Ý tưởng
Trò chơi bao gồm lọ và nước, có thể hình dung lọ như stack (theo LIFO) => sẽ coi mỗi lọ là 1 stack, mỗi level sẽ là 1 danh sách các stack, đưa nước thành màu về enums  
Nhận thấy game có các trạng thái chơi, thua, thắng -> sử dụng State Pattern -> tránh if, else lằng nhằng  
Mỗi lượt đổ nước tương đương như 1 command -> sử dụng Command Pattern -> với mỗi lượt là 1 command -> rõ ràng và có thể triển khai undo bằng cách lưu các command vào 1 danh sách  
Đồng thời dùng Observer pattern để tách UI ra khỏi logic  
Dùng Singleton cho GameManager, cố gắng tránh trường hợp GodObject vi phạm SOLID  

# Triển khai
Tham khảo: Rule of game design:  
RULE1: Tổng số màu phải chia hết
- Nếu mỗi tube chứa 4 slot, có 5 màu => mỗi màu phải xuất hiện 4 lần  

RULE2: Có ít nhất 2 tube rỗng  
-> đảm bảo level có thể giải được  

RULE3: Tube ban đầu hoàn chỉnh là rất hiếm (tránh)  
-> tránh trường hợp vào cái đã có lọ đã hoàn thành  

RULE4: Màu thường chia nhỏ, không chia đều  
-> các màu được chia đều/ ngẫu nhiên ở các lọ  

RULE5: TRÁNH dead-lock sớm  
-> tránh trường hợp hết lượt trong 1 vài lượt đầu  

RULE6: Quy tắc đổ màu  
Giả sử đổ từ A -> B (stack)  
- A phải không trống 
- B phải không đầy
- Màu đổ từ A sang B phải giống nhau ( A.Peek() là Blue thì B.Peek() cũng là Blue thì mới đổ được)
- Giả sử A có chứa Red, Blue, Blue thì đổ sang B, trong trường hợp còn đủ chỗ trống cho cả 2 thì phải đổ cả Blue, Blue; ngược lại nếu chỉ còn 1 thì đổ 1 Blue

# Từ ý tưởng và rule đặt ra, triển khai:
## 1. Xây dựng đối tượng
- TubeData bao gồm stack và depth (1 lọ chứa bao nhiêu màu) đại diện cho các lọ 
- LevelData bao gồm danh sách các TubeData và số lượng các lọ có trong màn
- enums ColorType đại diện cho nước

## 2. Design Pattern
- Xây dưng các state với State Pattern -> các state Play, Lose, Win có các hàm Enter() và Exit(), sau đó có thêm hàm CanHandleInput() trả về bool để trường hợp win hay lose không ấn vào lọ nữa
- Command Pattern -> PourCommand (mỗi hành động đổ nước là 1 command) có Excute() để thực thi và Undo()
- GameLogic (static class) -> triển khai các hàm core game (be) sử dụng xuyên suốt trò chơi:   
 + CanPour(TubeDate from -> to): kiểm tra có thể đổ được không: ý tưởng ở đây là "from" nếu không có gì hoặc "to" đầy -> không đổ; tương tự với phải màu top của stack của 2 thằng là giống nhau mới được đổ  
 + InCompleted(TubeData) -> stack mà chưa đầy (< depth) và các phần tử trong stack không giống thằng trên top -> là chưa hoàn thành lọ  
 + IsWin() -> kiểm tra 1 lượt nếu tất cả đã IsCompleted hoặc stack trống -> win  
 + HasAnyValidMOve() -> kiểm tra đôi 1 stack mà không có cái này CanPour() -> hết nước đi -> thua  
 !!! phần này khi làm về cuối em thấy có 1 vấn đề, em xin thể hiện luôn  
ở trường hợp 1  
1: red, blue <- top  
2: purple, purple, pink, pink  
3: yellow, blue, pink, red  
4: red, greeen, pink  
5: yellow, blue, green, red  
6: purple, blue, yellow, yellow  
7: green, green  
8: purple  
=> rõ ràng trường hợp này thì là thua do chỉ di chuyển được pink từ 2 qua 4 (nước đi vô nghĩa) -> nhưng đây vẫn tính là 1 nước đi hợp lệ -> không báo thua  
sửa lại sang trường hợp 2:  
1: purple, purple, yellow, pink <- top  
2: green, yellow, pink, blue  
3: pink, red, yellow  
4: green, blue, red, green  
5: red, purple, blue, pink  
6: green, yellow, blue, purple  
7: red  
8:   
=> logic chặt hơn khi 7 lọ đầu là không còn nước đi hợp lí -> báo thua luôn, NHƯNG không đúng vì còn có thể đẩy qua lọ 8 để tạo các nước đi hợp lí   
@@ cuối cùng em chốt quay lại phương án 1, bởi kiểu gì cũng còn nút Restart  
 + Revert() -> lùi lại 1 bước  
 + Pour(TubeDate from -> to, out poured) -> dùng vòng lặp while để đổ hết trên top stack màu cùng nhau sang stack khác (tất nhiên phải thỏa mãn CanPour()), con số poured ở đây là để lấy bao nhiêu màu giống nhau được chuyển đi -> được sử dụng để undo(): giả sử A{red, blue, blue}->B{blue} => poured = 2 thì khi undo từ B{blue, blue, blue} -> A{red} thì B pop() 2 lần là về ban đầu  

## 3. Màu
- Tạo ScriptableObject ColorSpriteDB để làm database cho màu, chứ ColorEntry chứa ColorType và Sprite -> mapping màu với enums, nhưng không phải tạo từng màu từng enums, mà sprite dùng chung, trong game thay đổi thông qua color của sprite là được; đồng thời có hàm Get() để lấy màu theo enums

## 4. Nhà máy xây dựnggggg
- Ý tưởng ban đầu em định dùng Factory Pattern để có thể tạo nhiều màn với nhiều kiểu chơi khác nhau nhưng sau đó để phù hợp thời gian và còn thời gian ôn thi vào hôm 6/12 -> chuyển sang static class -> ý tưởng có hàm tạo các lọ với tham số truyền vào là LevelData sẽ dynamic hơn  
- Nhưng còn 1 vấn đề ở đây là không chỉ be, cần fe với UI, câu hỏi là làm sao để có thể generate là số lọ mong muốn, các lọ cách đều, màu trong lọ thì trong hợp lí như trong game mà không phải cắt sprite ra
-> ý tưởng tới từ Sprite Mask (https://www.youtube.com/watch?v=4pl8DcsCQ_k)   
- Tạo 2 prefap, 1 là cho lọ với script là TubeView (thằng này là để hiển thị thôi), script này sẽ có trách nhiệm build mỗi lọ có đủ slot (là prefap thứ 2 chứa sprite) được truyền vào và cần có hàm Refresh() để làm mới UI các lọ khi đổ nước   
- TubeViewFactory() thằng này có nhiệm vụ gen ra các lọ đã được tạo ra từ TubeView với khoảng cách đều
- Cơ mà phải nhớ lại những RUle đặt ra từ trước, vậy cần đặt màu sao cho hợp lí, thay vì fill từ lọ, mình sẽ tạo 1 danh sách các màu cho vào các stack ấy rồi chia đều ra kiểu round-robin  
ý tưởng thêm màu ở đây là thêm lần lượt theo dạng Red, Red, Red, Red, Blue, Blue.... (với depth = 4 ) sau đó Shuffle và generate ra các lọ với đầy đủ màu trong lọ (đủ UI)  
- TubeView cũng kiêm luôn là MonoBehavior nhận event Bấm nút từ người chơi  
- Logic xếp màu trong các lọ (LevelGenerator)  
 + Khởi tạo LevelData với:  
  - numberOfTubes: tổng số lọ
  - depth: số slot màu trên mỗi lọ
  - emptyTubes: số lọ để trống ban đầu  
 + Từ colorCount và depth, em tạo một list màu “thô”:  
  - Ví dụ: colorCount = 5, depth = 4 → mỗi màu xuất hiện đúng 4 lần
  - Đảm bảo RULE1: tổng số màu chia hết và mỗi màu xuất hiện đều  
 List màu này được shuffle để tránh pattern lặp lại.  
 + Sau đó em fill vào các lọ có thể đổ (trừ emptyTubes), với một số constraint:  
  - Không đổ vào lọ đã đầy  
  - Hạn chế tạo lọ full ngay từ đầu với 1 màu duy nhất (giảm viễn cảnh RULE3)  
  - Hạn chế đổ liên tiếp cùng màu vào cùng lọ quá nhiều (giảm khả năng auto-completed tube từ đầu, và tạo mix màu “thật” hơn)  

* Cách tiếp cận này giữ được:  
- Đúng tổng số màu, đúng depth  
- Độ random đủ cao nhưng vẫn tuân thủ rule game  
- Hạn chế tối đa việc vừa vào game đã có lọ hoàn thành sẵn  

- Logic xếp màu trong các lọ (LevelGenerator)  
Khởi tạo LevelData với:
+ numberOfTubes: tổng số lọ trong level
+ depth: số slot màu tối đa trong mỗi lọ
+ emptyTubes: số lọ được để trống ban đầu
Từ colorCount và depth, tạo một danh sách màu ban đầu:  
+ Ví dụ: colorCount = 5, depth = 4 thì mỗi màu sẽ xuất hiện đúng 4 lần  
Đảm bảo tổng số màu luôn chia hết và mỗi màu xuất hiện đều (RULE1)  
Danh sách màu này được shuffle bằng thuật toán Fisher–Yates (tham khảo) để tránh sinh ra các pattern lặp lại hoặc dễ đoán  
Sau khi có danh sách màu đã trộn, tiến hành phân phối màu vào các lọ có thể chứa (loại trừ các lọ trống), với các ràng buộc sau:  
Không đổ màu vào lọ đã đầy  
Hạn chế tạo lọ hoàn chỉnh ngay từ đầu với chỉ một màu duy nhất để tránh có sẵn lọ hoàn thành (giảm khả năng vi phạm RULE3)  
Hạn chế việc đổ liên tiếp cùng một màu vào cùng một lọ quá nhiều, nhằm tạo sự phân tán màu hợp lý và tăng tính thử thách của level  

* Cách tiếp cận này đảm bảo:
- Tổng số màu chính xác và đúng giới hạn depth của mỗi lọ
- Mức độ random đủ cao nhưng vẫn tuân thủ luật của game
- Giảm tối đa trường hợp vừa vào level đã có những lọ hoàn chỉnh sẵn

## 5. Triển khai Observer Pattern
Để tránh việc GameManager hoặc GameLogic phụ thuộc trực tiếp vào các thành phần UI trong scene, sử dụng một lớp static GameEvents đóng vai trò như một event bus trung gian  
- Các event chính trong hệ thống:  
+ OnPour: được gọi mỗi khi có hành động đổ màu giữa hai lọ 
+ OnTubeCompleted: được gọi khi một lọ đạt trạng thái hoàn chỉnh
+ OnWin: được gọi khi toàn bộ level đạt điều kiện thắng
+ OnLose: được gọi khi không còn nước đi hợp lệ
+ OnUndo: được gọi khi người chơi thực hiện thao tác undo
- Các lớp phát event:  
+ PourCommand: 
Sau khi thực hiện logic đổ màu trong GameLogic, phát event OnPour  
Nếu lọ đích trở thành lọ hoàn chỉnh, phát thêm event OnTubeCompleted  
+ WinState:
Khi enter state thắng, phát event OnWin  
+ LoseState:
Khi enter state thua, phát event OnLose  
- Các lớp lắng nghe event:
+ TubeView:  
Đăng ký lắng nghe các event OnPour, OnTubeCompleted và OnUndo  
Khi xảy ra OnPour và dữ liệu liên quan đến lọ hiện tại, gọi Refresh để đồng bộ UI  
Khi xảy ra OnTubeCompleted, hiển thị nắp lọ và hiệu ứng tương ứng  
+ WinPopup và LosePopup:  
Lắng nghe event OnWin hoặc OnLose  
Chỉ chịu trách nhiệm bật panel UI tương ứng, không chứa logic game  

* Nhờ Observer Pattern:  
Core gameplay (GameManager, GameLogic, Command) không phụ thuộc trực tiếp vào UI  
UI có thể thay đổi, animate hoặc mở rộng hiệu ứng mà không cần chỉnh sửa logic gam  
Gameplay logic dễ test và dễ bảo trì hơn vì không bị trộn lẫn với code hiển thị  

## 6. Difficulty
- Để dễ mở rộng nhiều level/mode mà không phải hard-code từng layout, em thêm một lớp DifficultyConfig và dùng enum  
public struct DifficultyConfig  
{  
    public int numberOfTubes;  
    public int emptyTubes;  
    public int depth;  
    public int colorCount;  
}  
Đồng thời setup sẵn các lọ phụ thuộc mức độ khó dễ cho từng enum  
- GameManager chỉ cần nhận Difficulty từ Inspector: levelData = LevelGenerator.GenerateLevel(config);
  
* Cách này:
- Giữ LevelGenerator độc lập
- Dễ chỉnh số liệu difficulty mà không ảnh hưởng game core

## 7. Luồng game tóm gọn
- Khi bắt đầu game:
GameManager khởi tạo level mới thông qua LevelGenerator dựa trên độ khó đã chọn  
LevelData được tạo ra, chứa danh sách TubeData (các lọ và màu tương ứng)  
TubeViewFactory sinh ra các TubeView trong scene và bind mỗi TubeView với một TubeData  
Game chuyển sang trạng thái PlayState, cho phép người chơi tương tác  
- Người chơi bắt đầu thao tác:
Người chơi click vào một lọ  
GameManager kiểm tra state hiện tại có cho phép input hay không  
Nếu hợp lệ, lọ được chọn làm source và được highlight  
- Người chơi click sang lọ khác:
GameManager gọi GameLogic.CanPour để kiểm tra có thể đổ hay không  
Nếu không hợp lệ:
Bỏ chọn source hiện tại  
Cho phép chọn lại lọ khác  
- Nếu hợp lệ:
Tạo một PourCommand mới  
Gọi Execute để thực hiện logic đổ màu trong GameLogic  
Lưu command vào history để phục vụ undo  
- Khi đổ màu thành công:
PourCommand phát event OnPour thông qua GameEvents  
Các TubeView liên quan nhận event và gọi Refresh để cập nhật giao diện  
AudioManager phát âm thanh đổ nước  
- Nếu lọ đích hoàn thành:
PourCommand phát thêm event OnTubeCompleted  
TubeView hiển thị nắp lọ và hiệu ứng hoàn thành  
AudioManager phát âm thanh hoàn thành  
- Sau mỗi lượt:
GameManager kiểm tra điều kiện thắng:
Nếu tất cả các lọ non-empty đều ở trạng thái hoàn thành → chiến thắng  
Game chuyển sang WinState  
Phát event OnWin  
WinPopup hiển thị màn hình chiến thắng  
Nếu chưa thắng, tiếp tục cho người chơi thao tác trong PlayState  
- Trường hợp undo trong khi đang chơi:  
Người chơi nhấn nút Undo  
GameManager pop command gần nhất khỏi history  
Gọi Undo trên command để hoàn tác một lượt đổ  
UI tự động cập nhật thông qua event OnUndo  
Game quay lại PlayState và tiếp tục chơi  
- Kết thúc happy case:  
Người chơi giải thành công toàn bộ level  
Game hiển thị WinPopup  
Input bị khoá, tránh thao tác thêm sau khi thắng  
Người chơi có thể chọn chơi lại hoặc quay về menu  

# Có thể triển khai thêm nếu có thời gian
- Em đang tìm hiểu thêm về dotween, em mới chỉ có thể làm đơn giản với dotween thôi ạ, game với hiệu ứng đẹp sẽ thu hút hơn
- Sử dụng factory pattern kết hợp với độ khó có thể setting (hiện tại trong inspector) được để có thể tạo nhiều màn chơi với cách chơi khác 
- 1 số ý tưởng mới: 
 + kiểu túi mù: đổ lớp màu trên thì mới thấy ở dưới
 + tính thời gian cho màn chơi
