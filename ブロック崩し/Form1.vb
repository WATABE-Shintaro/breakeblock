'Option Strict On

Public Class MForm

    ''' <summary>右矢印が押されているか</summary>
    Private KRight As Boolean
    ''' <summary>左矢印が押されているか</summary>
    Private KLeft As Boolean

    Private RandomObject As System.Random   'ランダム用変数

    Private ball1 As Ball           'tempボール１
    Private f As Boolean            'temp ぼ－るがあるか
    Private Bcount As Integer = 0   'ボールがいくつあるか
    Private ball(100) As Ball       'ボール群
    Private b(100) As Boolean       'ボールがあるか
    Private Gcount As Integer       'ブロックがいくつあるか
    Private blocks(100) As Block    'ブロック群
    Private g(100) As Boolean       'ブロックがあるか
    Private score As Integer        'ゲームのスコア
    Private tempscore As Integer    '壊したブロック
    Private missscore As Integer    'ミス回数
    Private totalscore As Integer   'トータルスコア
    Private inGame As Integer       'ゲーム中かどうか
    Private gametype As Integer     'ゲームの種類

    Private Const MYLeftest = 10        '左の壁
    Private Const MYRightest = 740      '右の壁
    Private Const MYHightest = 10       '上の壁
    Private Const MYBottomest = 550     '下の

    '板
    Private Const MYWidth = 100             '幅
    Private Const MYHight = 20              '厚み
    Private X As Integer = 360              '左端の位置（初期値）
    Private Y1 As Integer = 500             '上端位置（固定）
    Private X2 As Integer                   '右端の位置
    Private Y2 As Integer = 500 + MYHight   '下端の位置（固定）
    '板


    '初期動作＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        movetimer.Start()
        mymove()
    End Sub
    Public Sub New()

        ' この呼び出しは、Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。
        RandomObject = New System.Random
        PictureBox2.Dispose()
        PictureBox3.Dispose()
        messagelabel.Text = Nothing
    End Sub

    '操作＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    Private Sub Form1_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown

        Select Case e.KeyCode
            Case Keys.Right
                KRight = True
            Case Keys.Left
                KLeft = True
        End Select

    End Sub
    Private Sub Form1_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyUp

        Select Case e.KeyCode
            Case Keys.Right
                KRight = False
            Case Keys.Left
                KLeft = False
            Case Keys.D
                inball(4)
            Case Keys.E
                inball()
            Case Keys.A
                If inGame Then inball()
            Case Keys.S
                Game1Begin()
            Case Keys.W
                Game2Begin()
        End Select

    End Sub

    '時間性動作＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    '時間ごとの板とボールの動作とチェック
    Private Sub movetimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles movetimer.Tick
        If KLeft Then
            If PictureBox1.Left >= MYLeftest + 5 Then
                X -= 5
                mymove()
            End If
        ElseIf KRight Then
            If PictureBox1.Left <= MYRightest - MYWidth - 5 Then
                X += 5
                mymove()
            End If
        End If
        For i = 0 To 100
            If b(i) Then ball(i).move()
        Next
        If f Then ball1.move()
        If f Then ballcheck2()
        ballcheck()
    End Sub

    '板の動作                                      ' from movetimer_Tick
    Private Sub mymove()
        PictureBox1.Left = X
        X2 = X + MYWidth
    End Sub

    'ボールのチェック         from movetimer_Tick
    Sub ballcheck2()

        '===============================================================================ボール１
        '画面の端に行った場合
        If ball1.Bottom > MYBottomest Then
            If f Then
                ball1.Dispose()
                f = False
                If inGame Then scoremake(False)
            End If
        End If
        If ball1.Top < MYHightest Then ball1.YBound()
        If ball1.Left < MYLeftest Then ball1.XBound()
        If ball1.right > MYRightest Then ball1.XBound()

        '板に当たったか
        If gametype = 1 Then
            If ball1.Bottom > Y1 And ball1.Top < Y1 Then
                If ball1.right > X And ball1.Left < X2 Then
                    ball1.YBound()
                    If KLeft Then
                        ball1.bectleX -= 1
                    ElseIf KRight Then
                        ball1.bectleX += 1
                    End If
                End If
            End If
        ElseIf gametype = 2 Then
            If ball1.Bottom > Y1 And ball1.Top < Y2 Then
                If ball1.right > X And ball1.Left < X2 Then
                    ball1.YBound()
                    If KLeft Then
                        ball1.bectleX -= 1
                    ElseIf KRight Then
                        ball1.bectleX += 1
                    End If
                End If
            End If
        End If

        'ブロックに当たったか
        For i = 0 To Gcount - 1
            If g(i) Then
                If ball1.Left < blocks(i).right And ball1.right > blocks(i).Left Then
                    If ball1.Bottom > blocks(i).Top And ball1.Top < blocks(i).Bottom Then
                        blocks(i).Destroy()
                        scoremake(True)
                        g(i) = False
                        If ball1.Top < blocks(i).Top Then
                            If ball1.bectleY > 0 Then
                                ball1.YBound()
                            End If
                        End If
                        If ball1.Bottom > blocks(i).Bottom Then
                            If ball1.bectleY < 0 Then
                                ball1.YBound()
                            End If
                        End If
                        If ball1.Left < blocks(i).Left Then
                            If ball1.bectleX > 0 Then
                                ball1.XBound()
                            End If
                        End If
                        If ball1.right > blocks(i).right Then
                            If ball1.bectleX < 0 Then
                                ball1.XBound()
                            End If
                        End If
                    End If
                End If
            End If
        Next
    End Sub
    Sub ballcheck()                           ' from movetimer_Tick
        '===================================================================ボール群
        For i2 = 0 To 100
            If b(i2) = True Then

                '画面の端に行った場合
                If ball(i2).Bottom > MYBottomest Then
                    ball(i2).Dispose()
                    b(i2) = False
                    If inGame Then scoremake(False)
                End If
                If ball(i2).Top < MYHightest Then ball(i2).YBound()
                If ball(i2).Left < MYLeftest Then ball(i2).XBound()
                If ball(i2).right > MYRightest Then ball(i2).XBound()

                '板に当たったか
                If gametype = 1 Then
                    If ball(i2).Bottom > Y1 And ball(i2).Top < Y1 Then
                        If ball(i2).right > X And ball(i2).Left < X2 Then
                            ball(i2).YBound()
                            If KLeft Then
                                ball(i2).bectleX -= 1
                            ElseIf KRight Then
                                ball(i2).bectleX += 1
                            End If
                        End If
                    End If
                ElseIf gametype = 2 Then
                    If ball(i2).Bottom > Y1 And ball(i2).Top < Y2 Then
                        If ball(i2).right > X And ball(i2).Left < X2 Then
                            ball(i2).YBound()
                            If KLeft Then
                                ball(i2).bectleX -= 1
                            ElseIf KRight Then
                                ball(i2).bectleX += 1
                            End If
                        End If
                    End If
                End If

                'ブロックに当たったか
                For i = 0 To Gcount - 1
                    If g(i) Then
                        If ball(i2).Left < blocks(i).right And ball(i2).right > blocks(i).Left Then
                            If ball(i2).Bottom > blocks(i).Top And ball(i2).Top < blocks(i).Bottom Then
                                blocks(i).Destroy()
                                scoremake(True)
                                g(i) = False
                                If ball(i2).Top < blocks(i).Top Then
                                    If ball(i2).bectleY > 0 Then
                                        ball(i2).YBound()
                                    End If
                                End If
                                If ball(i2).Bottom > blocks(i).Bottom Then
                                    If ball(i2).bectleY < 0 Then
                                        ball(i2).YBound()
                                    End If
                                End If
                                If ball(i2).Left < blocks(i).Left Then
                                    If ball(i2).bectleX > 0 Then
                                        ball(i2).XBound()
                                    End If
                                End If
                                If ball(i2).right > blocks(i).right Then
                                    If ball(i2).bectleX < 0 Then
                                        ball(i2).XBound()
                                    End If
                                End If
                            End If
                        End If
                    End If
                Next

            End If
        Next
    End Sub

    'ゲーム外操作＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    Private Sub 始めるToolStripMenuItem_Click(ByVal sender As System.Object, _
                             ByVal e As System.EventArgs) Handles 始めるToolStripMenuItem.Click
        Game1Begin()
    End Sub

    'ゲーム外動作＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    Private Sub Game1Begin()
        standby()
        gametype = 1
        Buildgame()
    End Sub
    Private Sub Game2Begin()
        standby()
        gametype = 2
        Buildgame()
    End Sub


    ''' <summary>
    ''' きれいにする
    ''' </summary>
    ''' <remarks></remarks>
    Sub standby()
        For i = 0 To Gcount - 1                 'ブロック消す
            blocks(i).GoDispose()
            g(i) = False
        Next
        Gcount = 0
        If f Then                               'ボール１消す
            ball1.Dispose()
            f = False
        End If
        For i = 0 To 100                        'ボール群消す
            If b(i) Then
                ball(i).Dispose()
                b(i) = False
            End If
        Next
        totalscore += score                     'スコア消す
        score = 0
        missscore = 0
        tempscore = 0
        ScoreLabel2.Text = score
        messagelabel.Text = Nothing
    End Sub
    'ゲーム開始
    Sub Buildgame()
        CntDwnTimer.Start()
        Cleateblocks(35, gametype)
        If gametype = 2 Then Panel1.BackColor = Color.Black
    End Sub
    'ゲームカウントダウン                                     from Buildgame (間接)
    Private Sub CntDwnTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CntDwnTimer.Tick
        Static CntDwn As Integer = 4
        CntDwn -= 1
        If CntDwn < 1 Then
            messagelabel.Text = Nothing
            inball()
            inGame = True
            CntDwnTimer.Stop()
            CntDwn = 4
        Else
            messagelabel.Text = CntDwn.ToString
        End If
    End Sub

    'ボール入れる
    Overloads Sub inball()
        If f = False Then
            ball1 = New Ball(400, 460, Panel1, gametype)
            ball1.bectleX = 2
            ball1.bectleY = -2
            f = True
        End If
    End Sub
    Overloads Sub inball(ByVal tbc As Integer, ByVal tX As Integer, ByVal tY As Integer, ByVal tbX As Integer, ByVal tbY As Integer)
        For i = Bcount To Bcount + tbc - 1
            If b(i) = False Then
                ball(i) = New Ball(tX, tY, Panel1, gametype)
                ball(i).bectleX = tbX
                ball(i).bectleY = tbY
                b(i) = True
            End If
        Next
        Bcount += tbc
    End Sub
    Overloads Sub inball(ByVal tbc As Integer)
        Dim ROFlag(6) As Boolean
        Dim Check As Boolean
        For i = 1 To tbc
            Check = False
            Do Until Check
                Dim RandomNumber As Integer = RandomObject.Next(-3, 3)
                If Not (ROFlag(RandomNumber + 3)) Then
                    inball(1, 400, 460, RandomNumber, -2)
                    ROFlag(RandomNumber + 3) = True
                    Check = True
                End If
            Loop
        Next
    End Sub
    'ブロック作成（暫定）
    Sub Cleateblocks(ByVal tgc As Integer, ByVal ttype As Integer)
        Gcount = tgc
        Dim sidecount As Integer
        Dim sidewidth As Integer
        'Dim 
        If gametype = 1 Then
            sidecount = 7

        End If
        For i = 0 To Gcount - 1
            g(i) = True
            Dim tempx As Integer = i
            Dim tempy As Integer = 0
            Do While tempx > sidecount - 1
                tempx -= sidecount
                tempy += 1
            Loop
            blocks(i) = New Block(15 + tempx * 100, 15 + tempy * 50, Panel1, ttype)

        Next
    End Sub

    'スコア                                       
    Sub scoremake(ByVal flag As Boolean)
        If flag Then
            tempscore += 1
        Else
            missscore += 1
        End If
        score = tempscore - missscore * 5
        ScoreLabel2.Text = score
        If tempscore = Gcount Then
            gameclear()
        End If
    End Sub
    'クリアしたとき                                 from scoremake
    Sub gameclear()
        messagelabel.Text = "GameClear!"
        inGame = False
    End Sub


End Class



''' <summary>
''' ボール
''' </summary>
''' <remarks></remarks>
Public Class Ball

    Implements IDisposable

    Private DX As Double
    Private DY As Double
    Private X As Integer
    Private Y As Integer
    Private X2 As Integer
    Private Y2 As Integer
    Private BallP As PictureBox
    Private Mytype As Integer
    Private Const Width1 As Integer = 20
    Private Const Hight1 As Integer = 20
    Private Const Width2 As Integer = 2
    Private Const Hight2 As Integer = 2
    Private Width As Integer
    Private Hight As Integer

    'Private Mothpanel As Panel
    ''' <summary>
    ''' ボールオブジェクトを作成します。
    ''' </summary>
    ''' <param name="tX">初期位置（X軸）</param>
    ''' <param name="tY">初期位置（Y軸）</param>
    ''' <param name="tpanel">これ（ピクチャーボックス）を貼り付けるパネル</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal tX As Integer, ByVal tY As Integer, ByVal tpanel As Panel, ByVal tmytype As Integer)
        Mytype = tmytype
        If Mytype = 1 Then
            Width = Width1
            Hight = Hight1
        ElseIf Mytype = 2 Then
            Width = Width2
            Hight = Hight2
        End If
        X = tX
        Y = tY
        BallP = New PictureBox
        With BallP
            .Left = X
            .Top = Y
            .Visible = True
            .BorderStyle = BorderStyle.None
            .Width = Width
            .Height = Hight
            If Mytype = 1 Then
                .BackColor = Color.Blue
            ElseIf Mytype = 2 Then
                .BackColor = Color.LightGreen
            End If
        End With
        'tpanel.BackColor = Color.Black
        'Mothpanel = tpanel
        tpanel.Controls.Add(BallP)
    End Sub

    '動作＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    Public Sub move()
        X += DX
        BallP.Left = X
        X2 = X + Width
        Y += DY
        BallP.Top = Y
        Y2 = Y + Hight
    End Sub
    Public Sub XBound()
        DX = -DX
    End Sub
    Public Sub YBound()
        DY = -DY
    End Sub

    'プロパティ＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    Public ReadOnly Property Top() As Integer
        Get
            Top = Y
        End Get
    End Property
    Public ReadOnly Property Bottom() As Integer
        Get
            Bottom = Y2
        End Get
    End Property
    Public ReadOnly Property Left() As Integer
        Get
            Left = X
        End Get
    End Property
    Public ReadOnly Property right() As Integer
        Get
            right = X2
        End Get
    End Property
    Public Property bectleX() As Double
        Get
            bectleX = DX
        End Get
        Set(ByVal tX As Double)
            DX = tX
        End Set
    End Property
    Public Property bectleY() As Double
        Get
            bectleY = DY
        End Get
        Set(ByVal tY As Double)
            DY = tY
        End Set
    End Property

    '消去＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    Private disposedValue As Boolean = False        ' 重複する呼び出しを検出するには

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                BallP.Dispose()
                ' TODO: 他の状態を解放します (マネージ オブジェクト)。
            End If

            ' TODO: ユーザー独自の状態を解放します (アンマネージ オブジェクト)。
            ' TODO: 大きなフィールドを null に設定します。
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class



''' <summary>
''' ブロック
''' </summary>
''' <remarks></remarks>
Public Class Block
    Implements IDisposable


    Private X As Integer
    Private Y As Integer
    Private X2 As Integer
    Private Y2 As Integer
    Private BreakImage(3) As Image
    Private WithEvents BreakTimer As Timer
    Private BlockP As PictureBox
    Private Mytype As Integer
    Private Const Width1 As Integer = 70
    Private Const Hight1 As Integer = 35
    Private Const Width2 As Integer = 2
    Private Const Hight2 As Integer = 2
    Private Width As Integer
    Private Hight As Integer

    ''' <summary>
    ''' ブロックオブジェクトを作成します。
    ''' </summary>
    ''' <param name="tX">初期位置（X軸）</param>
    ''' <param name="tY">初期位置（Y軸）</param>
    ''' <param name="tpanel">これ（ピクチャーボックス）を貼り付けるパネル</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal tX As Integer, ByVal tY As Integer, ByVal tpanel As Panel, ByVal tmytype As Integer)
        Mytype = tmytype

        If Mytype = 1 Then
            Width = Width1
            Hight = Hight1
        ElseIf Mytype = 2 Then
            Width = Width2
            Hight = Hight2
        End If

        X = tX
        Y = tY
        X2 = X + Width
        Y2 = Y + Hight
        BreakImage(0) = My.Resources.ブロック崩壊１
        BreakImage(1) = My.Resources.ブロック崩壊２
        BreakImage(2) = My.Resources.ブロック崩壊３
        BreakImage(3) = My.Resources.ブロック崩壊４
        BlockP = New PictureBox
        With BlockP
            .Left = X
            .Top = Y
            .Visible = True
            .BackColor = Color.Blue
            .Width = Width
            .Height = Hight
            If Mytype = 1 Then
                .BorderStyle = BorderStyle.FixedSingle
            ElseIf Mytype = 2 Then
                .BorderStyle = BorderStyle.None
            End If
        End With
        'tpanel.BackColor = Color.Black
        'Mothpanel = tpanel
        tpanel.Controls.Add(BlockP)

        BreakTimer = New Timer
        BreakTimer.Interval = 150
    End Sub

    'プロパティ＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    Public ReadOnly Property Top() As Integer
        Get
            Top = Y
        End Get
    End Property
    Public ReadOnly Property Bottom() As Integer
        Get
            Bottom = Y2
        End Get
    End Property
    Public ReadOnly Property Left() As Integer
        Get
            Left = X
        End Get
    End Property
    Public ReadOnly Property right() As Integer
        Get
            right = X2
        End Get
    End Property


    '消去＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    Private Sub BT(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BreakTimer.Tick
        Static i As Integer = 0
        If i = 0 Then
            With BlockP
                .Width = Width + 10
                .Height = Hight + 10
                .Left = X - 5
                .Top = Y - 5
                .BorderStyle = BorderStyle.None
            End With
        End If
        BlockP.Image = BreakImage(i)
        i += 1
        If i = 4 Then
            GoDispose()
            BreakTimer.Stop()
            i = 0
        End If
    End Sub

    ''' <summary>
    ''' ブロックを壊します
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Destroy()
        If Mytype = 1 Then
            Static dv As Boolean = True
            If dv Then BreakTimer.Start()
            dv = False
        Else
            GoDispose()
        End If
    End Sub
    ''' <summary>
    ''' ブロックを消去します
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub GoDispose()
        BreakTimer.Dispose()
        BlockP.Dispose()
        Me.Dispose()
    End Sub

    Private disposedValue As Boolean = False        ' 重複する呼び出しを検出するには

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: 他の状態を解放します (マネージ オブジェクト)。
            End If

            ' TODO: ユーザー独自の状態を解放します (アンマネージ オブジェクト)。
            ' TODO: 大きなフィールドを null に設定します。
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class



