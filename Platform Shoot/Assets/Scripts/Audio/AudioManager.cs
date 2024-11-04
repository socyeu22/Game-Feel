using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.PlayerLoop;


public class AudioManager : MonoBehaviour
{
    [Range(0f, 2f)]
    [SerializeField] private float _masterVolume = 1f;// Biến dùng để điều chỉnh âm lượng của âm thanh chung
    [SerializeField] private SoundCollectionSO _soundCollection; // Biến này dùng để lưu trữ các âm thanh (có thể là các chuỗi âm thanh) để phát cho các sự kiện khác nhau
    [SerializeField] private AudioMixerGroup _sfxMixerGroup; // Biến AudioMixerGroup dùng để lưu trữ nhóm mixer cho âm thanh SFX(dùng để điều chỉnh ở inspector)
    [SerializeField] private AudioMixerGroup _musicMixerGroup; // Biến AudioMixerGroup dùng để lưu trữ nhóm mixer cho âm thanh Music(dùng để điều chỉnh ở inspector)

    private AudioSource _currentMusic; // Biến này dùng để lưu âm thanh của nhạc đang phát(không phải âm thanh ngắn như sfx), dùng để kiểm soát chế độ nhạc FightMusic và DiscoBallMusic, chỉ dùng khi loại AudioType là Music

    #region Unity Methods
    private void Start() {
        FightMusic(); // Phát nhạc FightMusic ngay khi bắt đầu game
    }

    private void OnEnable() {
        Gun.OnShoot += Gun_OnShoot; // Đăng ký quan sát cho hàm Gun_OnShoot khi súng bắn
        PlayerController.OnJump += PlayerController_OnJump; // Đăng ký quan sát cho hàm PlayerController_OnJump khi nhân vật nhảy
        Health.OnDeath += Health_OnDeath; // Đăng ký quan sát cho hàm Health_OnDeath
        DiscoBallManager.OnDiscoBallHitEvent += DiscoBallMusic; // Đăng ký quan sát cho hàm DiscoBallMusic khi bóng disco va chạm
    }

    private void OnDisable() {
        Gun.OnShoot -= Gun_OnShoot;
        PlayerController.OnJump -= PlayerController_OnJump;
        Health.OnDeath -= Health_OnDeath;
        DiscoBallManager.OnDiscoBallHitEvent -= DiscoBallMusic;
    }
    #endregion

    #region Sound Methods
    // Hàm phát âm thanh ngẫu nhiên từ mảng âm thanh được truyền vào, hàm này dùng để tạo âm thanh ngẫu nhiên cho mỗi sự kiện, tạo cảm giác mới lạ, chân thực hơn
    private void PlayRandomSound(SoundSO[] sounds) { // Nhận vào một mảng âm thanh(cho cùng một sự kiện)
        if(sounds != null && sounds.Length > 0) {
            SoundSO soundSO = sounds[Random.Range(0, sounds.Length)];
            SoundToPlay(soundSO); // Gọi hàm SoundToPlay để phát âm thanh
        }

    }

    // Hàm phát âm thanh, nhận vào một âm thanh cụ thể để phát theo các thông số của âm thanh đó và AudioMixerGroup tương ứng
    private void SoundToPlay(SoundSO soundSO)
    {
        /*
        Các thông số của âm thanh được lấy từ SoundSO, sau đó sẽ được truyền vào hàm PlaySound để phát âm thanh
        AudioMixerGroup sẽ được chọn dựa vào loại âm thanh được chọn từ SoundSO
        */
        AudioClip clip = soundSO.Clip; // Lấy clip âm thanh từ SoundSO
        float pitch = soundSO.Pitch; // Lấy pitch âm thanh từ SoundSO
        float volume = soundSO.Volume * _masterVolume; // Lấy âm lượng từ SoundSO và hiệu chỉnh theo âm lượng chung
        bool loop = soundSO.Loop; // Lấy giá trị loop từ SoundSO
        AudioMixerGroup audioMixerGroup; // Biến này dùng để lưu trữ AudioMixerGroup tương ứng với loại âm thanh

        // Thiết lập pitch ngẫu nhiên nếu RandommizePitch được chọn(pitch là cao độ của âm thanh)
        pitch = RadomizePitch(soundSO, pitch);

        // Chọn AudioMixerGroup tương ứng với loại âm thanh
        audioMixerGroup = DeterminAudioMixerGroup(soundSO);

        // Gọi hàm PlaySound để phát âm thanh với những tham số âm thanh và AudioMixerGroup đã được chọn
        PlaySound(clip, pitch, volume, loop, audioMixerGroup);
    }

    // Hàm này dùng để chọn AudioMixerGroup tương ứng với loại âm thanh được chọn từ SoundSO
    private AudioMixerGroup DeterminAudioMixerGroup(SoundSO soundSO)
    {
        AudioMixerGroup audioMixerGroup;
        switch (soundSO.AudioType)
        {
            case SoundSO.AudioTypes.SFX: // Nếu loại âm thanh là SFX thì chọn AudioMixerGroup là _sfxMixerGroup
                audioMixerGroup = _sfxMixerGroup;
                break;
            case SoundSO.AudioTypes.Music: // Nếu loại của âm thanh là Music thì chọn AudioMixerGroup là _musicMixerGroup
                audioMixerGroup = _musicMixerGroup;
                break;
            default:
                audioMixerGroup = null;
                break;

        }

        return audioMixerGroup;
    }

    // Hàm này dùng để random pitch của âm thanh nếu RandommizePitch được chọn
    private static float RadomizePitch(SoundSO soundSO, float pitch)
    {
        if (soundSO.RandommizePitch)
        {
            float randomPitchModifier = Random.Range(-soundSO.RandomPitchRangeModifier, soundSO.RandomPitchRangeModifier);
            pitch = soundSO.Pitch + randomPitchModifier;
        }

        return pitch;
    }

    // private void PlaySound(SoundSO sound) {
    //     GameObject soudObject = new GameObject("Temp Audio Source");
    //     AudioSource audioSource = soudObject.AddComponent<AudioSource>();
    //     audioSource.clip = sound.Clip;
    //     audioSource.Play();
    // }

    // Hàm phát âm thanh, nhận vào clip âm thanh, pitch, volume, loop, AudioMixerGroup để phát âm thanh
    private void PlaySound(AudioClip clip, float pitch, float volume, bool loop, AudioMixerGroup audioMixerGroup)
    {
        // Tạo một game obj mới để phát âm thanh
        GameObject soudObject = new GameObject("Temp Audio Source"); // Tạo game obj mới
        AudioSource audioSource = soudObject.AddComponent<AudioSource>(); // Thêm component AudioSource(dùng để phát âm thanh) vào game obj mới đó

        // Thiết lập các thông số cho audioSource
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.outputAudioMixerGroup = audioMixerGroup; // Chọn AudioMixerGroup cho audioSource(AudioMixerGroup sẽ được quy định bởi AudioType của SoundSO)
        audioSource.Play(); // Phát âm thanh
        // Hủy game obj sau khi phát xong nếu không được lặp
        if (!loop)
        {
            Destroy(soudObject, clip.length); // Huỷ game obj khi hết thời gian phát của âm thanh(clip.length là độ dài của âm thanh)
        }

        // Kiếm tra nếu kiểu của âm thanh(soundSO) là music thì chỉ phát một âm thanh của loại đó tại một thời điểm
        DeterminMusic(audioMixerGroup, audioSource);
    }

    // Hàm này dùng để kiểm tra nếu âm thanh đang phát là âm thanh music thì dừng âm thanh đó và gán âm thanh hiện tại là âm thanh đó
    private void DeterminMusic(AudioMixerGroup audioMixerGroup, AudioSource audioSource)
    {
        if (audioMixerGroup == _musicMixerGroup)
        {
            if (_currentMusic != null)
            { // Nếu đang phát nhạc thì dừng nhạc đó, vì music chỉ phát một âm thanh tại một thời điểm(FightMusic hoặc DiscoBallMusic)
                _currentMusic.Stop(); // Dừng muisc đang phát trước đó
            }

            _currentMusic = audioSource; // Gán muisc hiện tại là audioSource vừa phát
        }
    }
    #endregion

    #region  SFX
    // private void Gun_OnShoot() {
    //     PlaySound(_gunShoot);
    // }
    // private void PlayerController_OnJump() {
    //     PlaySound(_jump);
    // }


    // Hàm được gọi khi súng bắn(dùng để phát âm thanh bắn của súng)
    private void Gun_OnShoot() {
        PlayRandomSound(_soundCollection.GunShoot);
    }

    // Hàm được gọi khi nhân vật nhảy(dùng để phát âm thanh nhảy)
    private void PlayerController_OnJump() {
        PlayRandomSound(_soundCollection.Jump);
    }
    // Hàm được gọi khi nhân vật die(dùng để phát âm thanh Splater)
    private void Health_OnDeath(Health health) {
        PlayRandomSound(_soundCollection.Splat);
    }
    #endregion

    #region Music
    // Hàm được gọi khi ở chế độ FightMusic(sẽ đươc lặp trong cài đặt SoundSO)
    private void FightMusic() {
        PlayRandomSound(_soundCollection.FightMusic);
    }

    // Hàm được gọi khi ở chế độ DiscoBallMusic(Không được lặp trong cài đặt SoundSO)
    private void DiscoBallMusic() {
        PlayRandomSound(_soundCollection.DiscoParty); // Phát âm thanh DiscoParty
        float soundLength = _soundCollection.DiscoParty[0].Clip.length; // Lấy độ dài của âm thanh DiscoParty(vì mảng DiscoParty chỉ chứa một âm thanh nên lấy âm thanh đầu tiên)
        // Invoke("FightMusic", soundLength); // Sau khi hết thời gian phát của DiscoParty thì chuyển về FightMusic
        Utils.RunAfterDelay(this, soundLength, FightMusic); // Sau khi hết thời gian phát của DiscoParty thì chuyển về FightMusic(sử dụng FightMusic như một hàm delegate action truyền vào hàm RunAfterDelay)
    }
    #endregion
}
