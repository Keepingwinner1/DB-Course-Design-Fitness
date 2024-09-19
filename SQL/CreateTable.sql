
CREATE TABLE "User" (
    "userID" NUMBER GENERATED BY DEFAULT AS IDENTITY,
    "userName" NVARCHAR2(50),
    "Password" VARCHAR2(50),
    "Email" VARCHAR2(100),
    "registrationTime" TIMESTAMP,
    "Age" NUMBER(3),
    "Gender" VARCHAR2(10),
    "isMember" NUMBER(1),
    "isPost" NUMBER(1),
    "isDelete" NUMBER(1),
    CONSTRAINT "PK_User" PRIMARY KEY ("userID") 
);


CREATE TABLE "CourseType" (
    "typeID" NUMBER GENERATED BY DEFAULT AS IDENTITY,
    "typeName" NVARCHAR2(20),
    CONSTRAINT "PK_CourseType" PRIMARY KEY ("typeID") 
);

CREATE TABLE "Course" (
    "classID" NUMBER GENERATED BY DEFAULT AS IDENTITY,
    "typeID" NUMBER,
    "courseName" NVARCHAR2(10),
    "Capacity" NUMBER(2),
    "courseDescription" NVARCHAR2(500),
    "coursePrice" NUMBER(4),
    "courseStartTime" TIMESTAMP,
    "courseEndtTime" TIMESTAMP,
    "courseLastDays" NUMBER(3),
    "courseGrade" NUMBER(2),
    "coursePhotoUrl" VARCHAR2(100),
    CONSTRAINT "PK_Course" PRIMARY KEY ("classID") ,
    CONSTRAINT "FK_Course_CourseType" FOREIGN KEY ("typeID") REFERENCES "CourseType" ("typeID") ON DELETE CASCADE
);

CREATE TABLE "Coach" (
    "coachID" NUMBER ,
    "userName" NVARCHAR2(20),
    "Age" NUMBER(3),
    "Gender" VARCHAR2(10),
    "iconURL" VARCHAR2(100),
    "isMember" NUMBER(1),
    "coachName" NVARCHAR2(10),
    CONSTRAINT "PK_Coach" PRIMARY KEY ("coachID") ,
    CONSTRAINT "FK_Coach_User" FOREIGN KEY ("coachID") REFERENCES "User" ("userID") ON DELETE CASCADE
);

CREATE TABLE "Trainee" (
    "traineeID" NUMBER,
    "userName" NVARCHAR2(20),
    "Age" NUMBER(3),
    "Gender" VARCHAR2(10),
    "iconURL" VARCHAR2(100),
    "traineeName" NVARCHAR2(10),
    "goalType" NVARCHAR2(10),
    "Duration" INTERVAL DAY TO SECOND,
    "goalWeight" NUMBER(3, 1),
    CONSTRAINT "PK_Trainee" PRIMARY KEY ("traineeID") ,
    CONSTRAINT "FK_Trainee_User" FOREIGN KEY ("traineeID") REFERENCES "User" ("userID") ON DELETE CASCADE
);

CREATE TABLE "Manager" (
    "managerID" NUMBER GENERATED BY DEFAULT AS IDENTITY,
    "managerName" NVARCHAR2(20),
    "Password" VARCHAR2(20),
    "Time" TIMESTAMP,
    CONSTRAINT "PK_Manager" PRIMARY KEY ("managerID") 
);

CREATE TABLE "Update" (
    "coachID" NUMBER,
    "classID" NUMBER,
    "actionType" NVARCHAR2(20),
    "updateTime" TIMESTAMP,
    "updateContext"  NVARCHAR2(20),
    CONSTRAINT "PK_Update" PRIMARY KEY ("coachID", "classID"),
    CONSTRAINT "FK_Update_Coach" FOREIGN KEY ("coachID") REFERENCES "Coach" ("coachID") ON DELETE CASCADE,
    CONSTRAINT "FK_Update_Course" FOREIGN KEY ("classID") REFERENCES "Course" ("classID") ON DELETE CASCADE
);

CREATE TABLE "ManagerUser" (
    "adminID" NUMBER,
    "userID" NUMBER,
    "Reason" NVARCHAR2(50),
    "Time" TIMESTAMP,
    CONSTRAINT "PK_ManagerUser" PRIMARY KEY ("adminID", "userID") ,
    CONSTRAINT "FK_ManagerUser_Manager" FOREIGN KEY ("adminID") REFERENCES "Manager" ("managerID") ON DELETE CASCADE,
    CONSTRAINT "FK_ManagerUser_User" FOREIGN KEY ("userID") REFERENCES "User" ("userID") ON DELETE CASCADE
);

CREATE TABLE "Teaches" (
    "coachID" NUMBER,
    "classID" NUMBER,
    "typeID" NUMBER,
    CONSTRAINT "PK_Teaches" PRIMARY KEY ("coachID", "classID"),
    CONSTRAINT "FK_Teaches_Coach" FOREIGN KEY ("coachID") REFERENCES "Coach" ("coachID") ON DELETE CASCADE,
    CONSTRAINT "FK_Teaches_Course" FOREIGN KEY ("classID") REFERENCES "Course" ("classID") ON DELETE CASCADE,
    CONSTRAINT "FK_Teaches_CourseType" FOREIGN KEY ("typeID") REFERENCES "CourseType" ("typeID") ON DELETE CASCADE
);

CREATE TABLE "Advise" (
    "classID" INT,
    "traineeID" INT,
    "coachID" INT,
    CONSTRAINT "PK_Advise" PRIMARY KEY ("classID", "traineeID"),
    CONSTRAINT "FK_Advise_Course" FOREIGN KEY ("classID") REFERENCES "Course"("classID") ON DELETE CASCADE,
    CONSTRAINT "FK_Advise_Trainee" FOREIGN KEY ("traineeID") REFERENCES "Trainee"("traineeID") ON DELETE CASCADE,
    CONSTRAINT "FK_Advise_Coach" FOREIGN KEY ("coachID") REFERENCES "Coach"("coachID") ON DELETE CASCADE
);

CREATE TABLE "Participate" (
    "traineeID" NUMBER,
    "classID" NUMBER,
    "typeID" NUMBER,
    "Grade" NUMBER(2),
    "Evaluate" NVARCHAR2(200),
    CONSTRAINT "PK_Participate" PRIMARY KEY ("traineeID", "classID") ,
    CONSTRAINT "FK_Participate_Trainee" FOREIGN KEY ("traineeID") REFERENCES "Trainee" ("traineeID") ON DELETE CASCADE,
    CONSTRAINT "FK_Participate_Course" FOREIGN KEY ("classID") REFERENCES "Course" ("classID") ON DELETE CASCADE,
    CONSTRAINT "FK_Participate_CourseType" FOREIGN KEY ("typeID") REFERENCES "CourseType" ("typeID") ON DELETE CASCADE
);

CREATE TABLE "Book" (
    "bookID" NUMBER GENERATED BY DEFAULT AS IDENTITY,
    "classID" NUMBER,
    "traineeID" NUMBER,
    "payMethod" CHAR(20),
    "bookStatus" NUMBER(1) DEFAULT 0,
    "bookTime" TIMESTAMP,
    CONSTRAINT "PK_Book" PRIMARY KEY ("bookID"),
    CONSTRAINT "FK_Book_Course" FOREIGN KEY ("classID") REFERENCES "Course" ("classID") ON DELETE CASCADE,
    CONSTRAINT "FK_Book_Trainee" FOREIGN KEY ("traineeID") REFERENCES "Trainee" ("traineeID") ON DELETE CASCADE
);

CREATE TABLE "Payment" (
    "paymentID" NUMBER GENERATED BY DEFAULT AS IDENTITY,
    "bookID" NUMBER,
    "Amount" NUMBER(10),
    "payMethod" CHAR(3),
    "paymentStatus" NUMBER(3) DEFAULT 0,
    "payTime" TIMESTAMP,
    CONSTRAINT "PK_Payment" PRIMARY KEY ("paymentID"),
    CONSTRAINT "FK_Payment_Book" FOREIGN KEY ("bookID") REFERENCES "Book" ("bookID") ON DELETE CASCADE
);

CREATE TABLE "Messages" (
    "messageID" NUMBER GENERATED BY DEFAULT AS IDENTITY,
    "senderID" NUMBER,
    "receiverID" NUMBER,
    "messageType" NVARCHAR2(10),
    "Content" NVARCHAR2(200),
    "sendTime" TIMESTAMP,
    CONSTRAINT "PK_Messages" PRIMARY KEY ("messageID") ,
    CONSTRAINT "FK_Messages_Sender" FOREIGN KEY ("senderID") REFERENCES "User" ("userID") ON DELETE CASCADE,
    CONSTRAINT "FK_Messages_Receiver" FOREIGN KEY ("receiverID") REFERENCES "User" ("userID") ON DELETE CASCADE
);

CREATE TABLE "Posts" (
    "postID"  NUMBER GENERATED BY DEFAULT AS IDENTITY,
    "userID" NUMBER,
    "postTitle" NVARCHAR2(20),
    "postContent" NVARCHAR2(500),
    "postCategory" NVARCHAR2(20),  
    "postTime" TIMESTAMP,
    "likesCount" NUMBER(10),
    "forwardCount" NUMBER(10),
    "commentsCount" NUMBER(10),
    CONSTRAINT "PK_Posts" PRIMARY KEY ("postID") ,
    CONSTRAINT "FK_Posts_User" FOREIGN KEY ("userID") REFERENCES "User" ("userID") ON DELETE CASCADE
);

CREATE TABLE "Publish" (
    "userID" NUMBER,
    "postID" NUMBER,
    "publishTime" TIMESTAMP,
    CONSTRAINT "PK_Publish" PRIMARY KEY ("userID", "postID"),
    CONSTRAINT "FK_Publish_User" FOREIGN KEY ("userID") REFERENCES "User" ("userID") ON DELETE CASCADE,
    CONSTRAINT "FK_Publish_Post" FOREIGN KEY ("postID") REFERENCES "Posts" ("postID") ON DELETE CASCADE
);

CREATE TABLE "Comment" (
    "commentID" NUMBER GENERATED BY DEFAULT AS IDENTITY,
    "userID" NUMBER NOT NULL,
    "postID" NUMBER NOT NULL,
    "parentCommentID" NUMBER DEFAULT NULL,
    "commentTime" TIMESTAMP,
    "likesCount" NUMBER(10),
    "Content" NVARCHAR2(200),
    CONSTRAINT "PK_Comment" PRIMARY KEY ("commentID") ,
    CONSTRAINT "FK_Comment_User" FOREIGN KEY ("userID") REFERENCES "User" ("userID") ON DELETE CASCADE,
    CONSTRAINT "FK_Comment_Post" FOREIGN KEY ("postID") REFERENCES "Posts" ("postID") ON DELETE CASCADE,
    CONSTRAINT "FK_Comment_ParentComment" FOREIGN KEY ("parentCommentID") REFERENCES "Comment" ("commentID") ON DELETE CASCADE
);

CREATE TABLE "ManagePost" (
    "adminID" NUMBER,
    "postID" NUMBER,
    "manageTime" TIMESTAMP,
    "deleteStatus" NUMBER(1),
    "promoteStatus" NUMBER(1),
    "restrictStatus" NUMBER(1),
    CONSTRAINT "PK_ManagePost" PRIMARY KEY ("adminID", "postID"),
    CONSTRAINT "FK_ManagePost_Admin" FOREIGN KEY ("adminID") REFERENCES "Manager" ("managerID") ON DELETE CASCADE,
    CONSTRAINT "FK_ManagePost_Post" FOREIGN KEY ("postID") REFERENCES "Posts" ("postID") ON DELETE CASCADE
);


CREATE TABLE "Friendship" (
        "friendshipID" NUMBER GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
        "userID" NUMBER,
        "friendID" NUMBER ,
        "createdTime" TIMESTAMP,
        CONSTRAINT "FK_User_Friend" FOREIGN KEY ("UserID") REFERENCES "User"("UserID"),
        CONSTRAINT "FK_Friend_User" FOREIGN KEY ("FriendID") REFERENCES "User"("UserID"),
        CONSTRAINT "UQ_User_Friend" UNIQUE ("UserID", "FriendID")
    );