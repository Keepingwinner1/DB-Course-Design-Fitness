<template>
  <notification-box />
  <common-layout />
  <div class="forum-bg">
    <el-backtop class="backtop-button" />
    <div class="back-button-container">
      <el-button @click="goBack" circle style="font-size: 24px; width: 50px; height: 50px;">
        <el-icon>
          <arrow-left />
        </el-icon>
      </el-button>
    </div>
    <div class="backHome-button-container">
      <el-button @click="goBackToHome" circle style="font-size: 24px; width: 50px; height: 50px;">
        <el-icon>
          <house />
        </el-icon>
      </el-button>
    </div>

    <!-- 相关帖子推荐 -->
    <el-card class="card">
      <div class="related-posts-section">
        <el-row class="row">
          <el-col :span="4">
            <icon-link />
          </el-col>
          <el-col :span="20">
            <el-text class="title">相关帖子推荐</el-text>
          </el-col>
        </el-row>
        <div class="related-posts-content">
          <el-divider class="related-post-divider" />
          <div v-for="post in relatedPosts" :key="post.postID" class="related-post-item">
            <icon-link class="icon-link-small" />
            <el-text @click="goToPost(post.postID)" class="related-post-title">
              {{ post.postTitle }}
            </el-text>
          </div>
        </div>
      </div>
    </el-card>

    <div class="post-container">
      <h1 class="post-title">{{ post.postTitle }}</h1>

      <div class="post-info">
        <span class="post-author" @click="goToAuthorProfile(post.userID)">{{ post.userName }}</span>
        <span class="post-time">{{ formatDate(post.postTime) }}</span>
      </div>

      <!-- 显示图片（如果存在） -->
      <div v-if="post.imgUrl != 'null'" class="post-image">
        <img :src="post.imgUrl" alt="Post Image" class="image" />
      </div>

      <div class="post-content-container">
        <div class="post-content">
          <!-- 使用 v-html 渲染内容以保留格式 -->
          <p v-html="post.postContent"></p>
        </div>
      </div>

      <el-divider class="post-divider"
                  style="border-width: 8px; border-color:#E1FFFF; background-color: 	#E1FFFF;"></el-divider>
      <div class="comments-section">
        <h3>评论区</h3>
        <div class="comments-container" ref="commentsContainer">
          <CommentItem
              v-for="comment in sortedComments"
              :key="comment.commentID"
              :comment="comment"
              @fetch-replies="fetchReplies"
              @like-comment="likeComment"
              @set-reply-target="setReplyTarget"
              @delete-comment="deleteComment"
          />
        </div>
        <!-- 回复目标显示 -->
        <div v-if="replyingTo" class="replying-to">
          <p>正在回复 @{{ replyingTo.userName }} 的评论：</p>
        </div>
        <!-- 切换容器的按钮 -->
        <button class="toggle-container-button" @click="toggleContainer">
          {{ isContainerVisible ? '收起评论栏' : '弹出评论栏' }}
        </button>
        <!-- 共同的容器 -->
        <transition name="slide-vertical">
          <div v-show="isContainerVisible" class="actions-container">
            <!-- 输入框和提交按钮 -->
            <div class="input-container fixed-input">
                            <textarea v-model="newCommentText" placeholder="写下你的评论..."
                                      @focus="clearReplyTarget"></textarea>
              <div class="actions">
                <button class="emoji-button" ref="emojiButton" @click="toggleEmojiPicker">😊</button>
                <button class="btn-primary" @click="addComment">发表评论</button>
              </div>
            </div>

            <!-- 帖子操作按钮 -->
            <div class="post-actions">
              <button @click="toggleLike(post.postID)" class="btn-action">
                👍 {{ postLiked ? '取消' : '点赞' }} {{ post.likesCount }}
              </button>
              <button @click="reportPost" class="btn-action">🚩 举报</button>
              <button @click="openShareDialog" class="btn-action">🔗 分享</button>
              <button @click="forwardPost" class="btn-action">🔄 转发</button>
              <button v-if="isCurrentUser(post.userName)" @click="deletePost(post.postID)"
                      class="btn-action">🗑 删除</button>

            </div>
          </div>
        </transition>
      </div>
    </div>

    <!-- 热帖推荐 -->
    <div class="right-sidebar">
      <div class="hot-posts-section">
        <el-row class="row">
          <el-col :span="4">
            <icon-fire />
          </el-col>
          <el-col :span="20">
            <el-text class="title">热帖推荐</el-text>
          </el-col>
        </el-row>
        <div class="hot-posts-content">
          <el-divider class="hot-post-divider" />
          <div v-for="hotPost in hotPosts" :key="hotPost.postID" class="hot-post-item">
            <icon-fire class="icon-fire-small" />
            <el-text @click="goToPost(hotPost.postID)" class="hot-post-title">
              {{ hotPost.postTitle }}
            </el-text>
          </div>
        </div>
      </div>
    </div>

    <!-- 分享弹窗 -->
    <el-dialog title="分享帖子" :visible="shareDialogVisible" width="30%" v-model="shareDialogVisible">
      <div>
        <p>复制下面的链接分享给他人：</p>
        <el-input v-model="shareLink" readonly></el-input>
      </div>
      <template #footer>
                <span class="dialog-footer">
                    <el-button @click="shareDialogVisible = false">关闭</el-button>
                    <el-button type="primary" @click="copyLink">复制链接</el-button>
                </span>
      </template>
    </el-dialog>

    <!-- 举报弹窗 -->
    <el-dialog title="确认举报" v-model="reportDialogVisible" width="30%">
      <div>
        <p>你确定要举报此帖子吗？</p>
      </div>
      <template #footer>
        <span class="dialog-footer">
            <el-button @click="reportDialogVisible = false">取消</el-button>
            <el-button type="danger" @click="confirmReport">确认举报</el-button>
        </span>
      </template>
    </el-dialog>

  </div>
</template>

<script>
import axios from 'axios';
import { ElNotification } from 'element-plus';
import { IconArrowLeft, IconFire, IconLink } from '@arco-design/web-vue/es/icon';
import { EmojiButton } from '@joeattardi/emoji-button';
import store from '../store/index.js';

import { commonMixin } from '../mixins/checkLoginState';
import CommentItem from '../components/CommentItem.vue';
import NotificationBox from '../components/NotificationBox.vue';

export default {
  mixins: [commonMixin],
  components: {
    IconArrowLeft,
    IconFire,
    IconLink,
    EmojiButton,
    CommentItem,
    NotificationBox
  },
  data() {
    return {
      showComments: true,
      newCommentText: "",
      replyingTo: null,
      currentUser: localStorage.getItem('name'),
      post: {
        postID: null,
        userID: null,
        userName: '',
        postTitle: '',
        postContent: '',
        postTime: '',
        likesCount: null,
        forwardCount: null,
        commentsCount: null,
        refrencePostID: null,
        imgUrl: '',
        isPinned: 0,
        isReported: 0,
      },
      postLiked: false,
      comments: [],
      relatedPosts: [],
      hotPosts: [],
      shareDialogVisible: false,
      reportDialogVisible: false,
      shareLink: "",
      isContainerVisible: true, // 容器初始显示状态
      hasCommentsNotification: false,  // 添加标志位
    };
  },
  mounted() {
    this.emojiPicker = new EmojiButton({
      position: 'bottom-start',
      zIndex: 9999,
    });
    this.emojiPicker.on('emoji', selection => {
      this.newCommentText += selection.emoji;
    });

    // 强制重新应用图标样式
    this.$nextTick(() => {
      document.querySelectorAll('.icon-fire-small').forEach(icon => {
        icon.style.fontSize = '10px';
        icon.style.width = '10px';
        icon.style.height = '10px';
      });
    });
  },
  computed: {
    // 根据 commentID 从大到小排序的 comments
    sortedComments() {
      return [...this.comments].sort((a, b) => b.commentID - a.commentID);
    }
  },
  watch: {
    '$route.params.postID': {
      handler(newVal, oldVal) {
        this.fetchPostDetail();
      },
      immediate: true
    }
  },

  created() {
    this.checkAvailable()
    this.fetchPostDetail();
    this.fetchRelatedPosts();
    this.fetchHotPosts();
    store.dispatch('pollIsPost');  // 开启轮询，更新发帖权限

  },
  methods: {
    deletePost(postID) {
      const token = localStorage.getItem('token');
      axios.delete(`http://localhost:8080/api/Post/DeletePostByPostID`, {
        params: {
          token: token,
          postID: postID,
          postOwnerID: this.post.userID
        }
      })
          .then(response => {
            console.log(response.data);
            if (response.data.message === '删除帖子成功') {
              ElNotification({
                title: '成功',
                message: '帖子删除成功',
                type: 'success',
              });
              this.$router.push('/forum'); // 删除后跳转回论坛首页
            } else {
              ElNotification({
                title: '错误',
                message: '删除帖子失败',
                type: 'error',
              });
            }
          })
          .catch(error => {
            ElNotification({
              title: '错误',
              message: '删除帖子时发生错误',
              type: 'error',
            });
          });
    },
    formatDate(date) {
      const d = new Date(date);
      const year = d.getFullYear();
      const month = String(d.getMonth() + 1).padStart(2, '0');
      const day = String(d.getDate()).padStart(2, '0');
      const hours = String(d.getHours()).padStart(2, '0');
      const minutes = String(d.getMinutes()).padStart(2, '0');
      const seconds = String(d.getSeconds()).padStart(2, '0');
      return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
    },

    isCurrentUser(userName) {
      return this.currentUser === userName || this.$store.state.role === 'admin';
    },
    toggleContainer() {
      this.isContainerVisible = !this.isContainerVisible; // 切换容器显示状态
      console.log('isContainerVisible:', this.isContainerVisible); // 调试输出
    },
    fetchPostDetail() {
      const token = localStorage.getItem('token');
      const postID = this.$route.params.postID;
      axios.get(`http://localhost:8080/api/Post/GetPostByPostID`, {
        params: {
          token: token,
          postID: postID
        }
      })
          .then(response => {
            console.log(response.data);
            this.post = response.data;
            this.fetchComments(postID);
            console.log("11",this.post.imgUrl);
          })
          .catch(error => {
            ElNotification({
              title: '错误',
              message: '获取帖子详情时发生错误',
              type: 'error',
            });
          });
    },
    fetchComments(postID) {
      const token = localStorage.getItem('token');
      axios.get(`http://localhost:8080/api/Comment/GetCommentByPostID`, {
        params: {
          token: token,
          postID: postID
        }
      })
          .then(response => {
            // 对评论数据按发表时间升序排列
            this.comments = response.data
                .map(comment => {
                  return {
                    ...comment,
                    likedByCurrentUser: false,
                    showReplies: false,
                    replies: [] // 确保replies数组存在
                  };
                })
                .sort((a, b) => new Date(a.commentTime) - new Date(b.commentTime));  // 按发表时间升序排列

            this.hasCommentsNotification = false; // 重置标志位
          })
          .catch(error => {
            if (error.response && error.response.status === 404) {
              this.comments = []; // 评论列表为空
              if (!this.hasCommentsNotification) {  // 检查是否已经提示过
                ElNotification({
                  title: '提示',
                  message: '该帖子暂无评论',
                  type: 'info',
                });
                this.hasCommentsNotification = true;  // 设置标志位为已提示
              }
            } else {
              ElNotification({
                title: '错误',
                message: '获取评论时发生错误',
                type: 'error',
              });
            }
          });
    },

    async fetchReplies(comment) {
      const token = localStorage.getItem('token');
      try {
        const response = await axios.get(`http://localhost:8080/api/Comment/GetCommentByCommentID`, {
          params: {
            token: token,
            commentID: comment.commentID
          }
        });
        const replies = response.data.filter(reply => reply.parentCommentID === comment.commentID).map(reply => {
          return {
            ...reply,
            likedByCurrentUser: false,
            replies: []
          };
        }).sort((a, b) => new Date(a.commentTime) - new Date(b.commentTime)); // 按时间排序
        comment.replies = replies;
      } catch (error) {
        if (error.response && error.response.status === 404) {
          // 处理404错误，假设表示没有回复
          ElNotification({
            title: '提示',
            message: '该评论暂无回复',
            type: 'info',
          });
        } else {
          ElNotification({
            title: '错误',
            message: '获取回复时发生错误',
            type: 'error',
          });
        }
      }
    },

    toggleReplies(comment) {
      if (!comment.showReplies) {
        this.fetchReplies(comment).then(() => {
          comment.showReplies = true;
        });
      } else {
        comment.showReplies = false;
      }
    },
    goBack() {
      this.$router.go(-1);
    },
    goBackToHome() {
      this.$router.push('/forum');
    },
    toggleLike(postID) {
      const token = localStorage.getItem('token');
      if (this.postLiked) {
        axios.get('http://localhost:8080/api/Post/CancleLikePost', {
          params: {
            token: token,
            postID: postID,
            postOwnerID: this.post.userID
          }
        })
            .then(() => {
              this.post.likesCount -= 1;
              this.postLiked = false;
              ElNotification({
                title: '成功',
                message: '已取消点赞',
                type: 'success',
              });
            })
            .catch(error => {
              ElNotification({
                title: '错误',
                message: '取消点赞时发生错误',
                type: 'error',
              });
            });
      } else {
        axios.get('http://localhost:8080/api/Post/LikePost', {
          params: {
            token: token,
            postID: postID,
            postOwnerID: this.post.userID
          }
        })
            .then(() => {
              this.post.likesCount += 1;
              this.postLiked = true;
              ElNotification({
                title: '成功',
                message: '点赞成功',
                type: 'success',
              });
            })
            .catch(error => {
              ElNotification({
                title: '错误',
                message: '点赞时发生错误',
                type: 'error',
              });
            });
      }
    },
    addComment() {
      const token = localStorage.getItem('token');
      if (this.newCommentText.trim()) {
        const newComment = {
          commentID: -1,
          userID: this.$store.state.userID,
          userName: localStorage.getItem('name'),
          postID: this.post.postID,
          parentCommentID: this.replyingTo ? this.replyingTo.commentID : -1,
          commentTime: new Date().toISOString(),
          likesCount: 0,
          content: this.newCommentText.trim()
        };

        if (this.replyingTo) {
          axios.post(`http://localhost:8080/api/Comment/ReplyComment?token=${token}`, newComment)
              .then(response => {
                if (response.data.message === '回复成功') {
                  this.newCommentText = ""; // 清空输入框
                  ElNotification({
                    title: '成功',
                    message: '回复成功',
                    type: 'success',
                  });
                  // 更新被回复的评论的回复列表
                  this.fetchReplies(this.replyingTo);
                  this.replyingTo = null; // 清除回复目标
                } else {
                  ElNotification({
                    title: '错误',
                    message: '回复失败',
                    type: 'error',
                  });
                }
              })
              .catch(error => {
                ElNotification({
                  title: '错误',
                  message: '回复时发生错误',
                  type: 'error',
                });
              });
        } else {
          // 处理一级评论发布的情况
          axios.post(`http://localhost:8080/api/Comment/PublishComment?token=${token}`, newComment)
              .then(response => {
                if (response.data.message === '发布评论成功') {
                  this.checkForAIComment(); // 检查并获取AI评论
                  this.newCommentText = ""; // 清空输入框
                  ElNotification({
                    title: '成功',
                    message: '评论发布成功',
                    type: 'success',
                  });
                  this.fetchComments(this.post.postID); // 重新获取评论列表
                } else {
                  ElNotification({
                    title: '错误',
                    message: '发布评论失败',
                    type: 'error',
                  });
                }
              })
              .catch(error => {
                ElNotification({
                  title: '错误',
                  message: '发表评论时发生错误',
                  type: 'error',
                });
              });
        }
      }
    },
    checkForAIComment() {
      let apiUrl = ''; // 定义一个变量来存储 API URL
      let userID = 0;
      let userName = '';
      if (this.newCommentText.includes('@健身教练')) {
        console.log('触发AI评论 - 健身教练');
        apiUrl = 'http://localhost:8080/api/Post/GetFitCoachComment';
        userID = 43;
        userName = "健身教练AI"
      } else if (this.newCommentText.includes('@营养顾问')) {
        console.log('触发AI评论 - 营养顾问');
        apiUrl = 'http://localhost:8080/api/Post/GetNutriExpertComment';
        userID = 44;
        userName = "营养顾问AI"
      } else if (this.newCommentText.includes('@激励导师')) {
        console.log('触发AI评论 - 激励导师');
        apiUrl = 'http://localhost:8080/api/Post/GetMotivatorComment';
        userID = 45;
        userName = "激励导师AI"
      }

      // 如果找到相应的@内容，调用API
      if (apiUrl) {
        axios.get(apiUrl, {
          params: {
            postTitle: this.post.postTitle, // 帖子标题
            postContent: this.post.postContent // 帖子内容
          }
        })
            .then(response => {
              if (response.data) {
                const aiComment = {
                  commentID: -1, // 可以在数据库插入后由后端返回实际ID
                  userID: userID,
                  userName: userName, // 你可以根据需要自定义名字
                  postID: this.post.postID,
                  parentCommentID: -1,
                  commentTime: new Date().toISOString(),
                  likesCount: 0,
                  content: response.data.message
                };

                // 向数据库插入AI评论
                axios.post(`http://localhost:8080/api/Comment/PublishComment?token=${localStorage.getItem('token')}`, aiComment)
                    .then(dbResponse => {
                      if (dbResponse.data.message === '发布评论成功') {
                        // 插入数据库成功后，将AI评论显示到前端
                        this.comments.push(aiComment);
                        this.fetchComments(this.post.postID); // 重新获取评论列表
                        ElNotification({
                          title: 'AI评论',
                          message: 'AI评论已生成并保存',
                          type: 'success',
                        });
                      } else {
                        ElNotification({
                          title: '错误',
                          message: '保存AI评论失败',
                          type: 'error',
                        });
                      }
                    })
                    .catch(error => {
                      ElNotification({
                        title: '错误',
                        message: '保存AI评论到数据库时发生错误',
                        type: 'error',
                      });
                    });
              }
            })
            .catch(error => {
              ElNotification({
                title: '错误',
                message: '获取AI评论时发生错误',
                type: 'error',
              });
            });
      }
    },
    likeComment(commentID) {
      const token = localStorage.getItem('token');
      const comment = this.comments.find(c => c.commentID === commentID) ||
          this.comments.flatMap(c => c.replies).find(r => r.commentID === commentID);

      if (!comment) {
        ElNotification({
          title: '错误',
          message: '评论未找到',
          type: 'error',
        });
        return;
      }

      if (comment.likedByCurrentUser) {
        // 取消点赞
        axios.get('http://localhost:8080/api/Comment/CancleLikeComment', {
          params: {
            token: token,
            commentID: commentID
          }
        })
            .then(response => {
              if (response.data === '取消点赞成功') {
                comment.likesCount--;
                comment.likedByCurrentUser = false;
                ElNotification({
                  title: '成功',
                  message: '取消点赞成功',
                  type: 'success',
                });
              } else {
                ElNotification({
                  title: '错误',
                  message: '取消点赞失败',
                  type: 'error',
                });
              }
            })
            .catch(error => {
              ElNotification({
                title: '错误',
                message: '取消点赞时发生错误',
                type: 'error',
              });
            });
      } else {
        // 点赞
        axios.get('http://localhost:8080/api/Comment/LikeComment', {
          params: {
            token: token,
            commentID: commentID
          }
        })
            .then(response => {
              if (response.data === '点赞成功') {
                comment.likesCount++;
                comment.likedByCurrentUser = true;
                ElNotification({
                  title: '成功',
                  message: '点赞成功',
                  type: 'success',
                });
              } else {
                ElNotification({
                  title: '错误',
                  message: '点赞失败',
                  type: 'error',
                });
              }
            })
            .catch(error => {
              ElNotification({
                title: '错误',
                message: '点赞时发生错误',
                type: 'error',
              });
            });
      }
    },

    // 递归删除评论及其子评论
    async deleteComment(commentID) {
      const token = localStorage.getItem('token');

      try
      {
        // 找到目标评论及其所在的父级评论数组（comments 或 replies）
        const findComment = (commentID, comments) =>
        {
          for (let i = 0; i < comments.length; i++)
          {
            if (comments[i].commentID === commentID)
            {
              return { comment: comments[i], parentArray: comments, index: i };
            }
            if (comments[i].replies && comments[i].replies.length > 0)
            {
              const result = findComment(commentID, comments[i].replies);
              if (result) return result;
            }
          }
          return null;
        };

        // 递归删除子评论，返回删除的评论总数
        const deleteRecursively = async (comment) =>
        {
          let deletedCount = 0;
          if (comment.replies && comment.replies.length > 0)
          {
            for (let reply of comment.replies)
            {
              deletedCount += await deleteRecursively(reply);
            }
          }

          // 删除当前评论
          const response = await axios.delete('http://localhost:8080/api/Comment/DeleteComment',
              {
                params:
                    {
                      token: token,
                      commentID: comment.commentID,
                      postID: this.post.postID
                    }
              });

          if (response.data === '评论删除成功')
          {
            deletedCount += 1;
          }
          else
          {
            throw new Error('删除评论失败');
          }

          return deletedCount;
        };

        const { comment, parentArray, index } = findComment(commentID, this.comments);

        if (comment)
        {
          // 先递归删除子评论
          const deletedCount = await deleteRecursively(comment);

          // 从父级数组中移除当前评论
          parentArray.splice(index, 1);

          // 同步更新评论总数
          this.post.commentsCount -= deletedCount;

          ElNotification({
            title: '成功',
            message: '评论删除成功',
            type: 'success',
          });
        } else
        {
          ElNotification({
            title: '错误',
            message: '未找到要删除的评论',
            type: 'error',
          });
        }
      } catch (error) {
        console.log(error);
        ElNotification({
          title: '错误',
          message: '删除评论时发生错误',
          type: 'error',
        });
      }
    },


    setReplyTarget(comment) {
      this.replyingTo = comment;
      this.newCommentText = `@${comment.userName} `;

    },
    clearReplyTarget() {
      if (!this.newCommentText.trim()) {
        this.replyingTo = null;
      }
    },
    openShareDialog() {
      this.shareLink = `${window.location.origin}/post/${this.post.postID}`;
      this.shareDialogVisible = true;
    },
    copyLink() {
      navigator.clipboard.writeText(this.shareLink).then(() => {
        ElNotification({
          title: '成功',
          message: '链接已复制到剪贴板！',
          type: 'success',
        });
      });
    },
    reportPost() {
      this.reportDialogVisible = true; // 打开举报弹窗
    },
    confirmReport() {
      const token = localStorage.getItem('token');
      axios.get(`http://localhost:8080/api/Post/ReportPost`, {
        params: {
          token: token,
          postID: this.post.postID
        }
      })
          .then(response => {
            if (response.data.message === '成功举报') {
              ElNotification({
                title: '成功',
                message: '举报成功，感谢你的反馈。',
                type: 'success',
              });
              this.post.isReported = 1; // 更新isReported标志位
              this.$router.push('/forum'); // 跳转回论坛页面
            } else {
              ElNotification({
                title: '错误',
                message: '举报失败，请稍后再试。',
                type: 'error',
              });
            }
          })
          .catch(error => {
            console.log(error);
            ElNotification({
              title: '错误',
              message: '举报时发生错误，请稍后再试。',
              type: 'error',
            });
          });
      this.reportDialogVisible = false; // 关闭举报弹窗
    },
    goToAuthorProfile(userID) {
      const token = localStorage.getItem('token');
      axios.get(`http://localhost:8080/api/User/GetProfile`, {
        params: {
          token: token,
          userID: userID,
        }
      })
          .then(response => {
            console.log(response.data);
            this.$router.push(`/user/${response.data.userID}`);
          })
          .catch(error => {
            ElNotification({
              title: '错误',
              message: '获取用户信息失败',
              type: 'error',
            });
          });
      //this.$router.push(`/user/${this.post.userID}`);
    },
    forwardPost() {
      const token = localStorage.getItem('token');
      const postID = this.$route.params.postID;
      axios.get(`http://localhost:8080/api/Post/ForwardPost`, {
        params: {
          token: token,
          postID: postID
        }
      })
          .then(response => {
            if (response.data.message === '成功转发') {
              this.post.forwardCount++;
              ElNotification({
                title: '成功',
                message: '帖子转发成功',
                type: 'success',
              });
            } else {
              ElNotification({
                title: '错误',
                message: '转发帖子失败',
                type: 'error',
              });
            }
          })
          .catch(error => {
            ElNotification({
              title: '错误',
              message: '转发帖子时发生错误',
              type: 'error',
            });
          });
    },
    fetchRelatedPosts() {
      const token = localStorage.getItem('token');
      axios.get(`http://localhost:8080/api/Post/GetAllPost?token=${token}`)
          .then(response => {
            const numberOfPosts = Math.floor(Math.random() * 6) + 6;
            const allPosts = response.data;
            this.relatedPosts = allPosts.sort(() => 0.5 - Math.random()).slice(0, numberOfPosts);
            console.log("获取相关帖子成功")
          })
          .catch(error => {
            ElNotification({
              title: '错误',
              message: '获取相关帖子时发生错误',
              type: 'error',
            });
          });
    },
    fetchHotPosts() {
      const token = localStorage.getItem('token');
      axios.get(`http://localhost:8080/api/Post/GetAllPost?token=${token}`)
          .then(response => {
            const allPosts = response.data;
            this.hotPosts = allPosts
                .sort((a, b) => (b.likesCount + b.commentsCount) - (a.likesCount + a.commentsCount))
                .slice(0, 5);
            console.log("获取热帖成功");
          })
          .catch(error => {
            ElNotification({
              title: '错误',
              message: '获取热帖时发生错误',
              type: 'error',
            });
          });
    },
    goToPost(postID) {
      // 更新路由参数，跳转到新帖子
      this.$router.push(`/post/${postID}`).then(() => {
        // 重新获取帖子详情和评论
        this.fetchPostDetail();
      });
    },
    toggleEmojiPicker() {
      // 如果表情选择器已经打开，恢复页面的滚动状态
      document.body.style.overflow = this.emojiPicker.isOpen ? '' : 'hidden';
      this.emojiPicker.togglePicker(this.$refs.emojiButton);

      // 确保当表情选择器关闭时，恢复页面滚动
      if (!this.emojiPicker.isOpen) {
        document.body.style.overflow = '';  // 允许页面滚动
      }
    },

    highlightCommentAction(event) {
      event.target.style.backgroundColor = '#f0f0f0';
      event.target.style.cursor = 'pointer';
    },
    resetCommentAction(event) {
      event.target.style.backgroundColor = '';
      event.target.style.cursor = '';
    },
    beforeDestroy() {
      document.body.style.overflow = '';
    },
  },
}
</script>

<style scoped>
.forum-bg {
  display: flex;
  background-image: url('../assets/images/forum-bg.jpg');
  background-size: cover;
  background-position: center;
  background-attachment: fixed;
  width: 100%;
  min-height: 100vh;
  position: absolute;
  top: 0;
  left: 0;
  padding-bottom: 60px;
  overflow-y: auto;
}

.post-container {
  display: flex;
  flex-direction: column;
  width: 700px;
  margin: 0 auto;
  background-color: transparent;
  border: none;

}

.post-title {
  margin-top: 20px;
  text-align: center;
  font-weight: bold;
  font-size: 30px;
  margin-bottom: 20px;
  color: #333;
}

.post-info {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 50px;
  font-size: 14px;
  color: #777;
  margin-top: 5px;
  margin-bottom: 10px;
}

.post-info span {
  display: inline-flex;
  align-items: center;
}

.post-author {
  color: #007bff;
  cursor: pointer;
}

.post-content {
  padding: 20px;
  margin-bottom: 30px;
  font-size: 16px;
  line-height: 1.6;
  color: #444;
  overflow-wrap: break-word
}

.post-actions {
  position: fixed;
  bottom: 0;
  left: 0;
  width: 100%;
  background-color: transparent;
  border: none;
  display: flex;
  justify-content: center;
  gap: 20px;
  padding: 10px 0;
  z-index: 100;
}

.btn-action {
  background-color: #007bff;
  color: white;
  padding: 10px 20px;
  border-radius: 10px;
  cursor: pointer;
  font-size: 14px;
  border: none;
}

.btn-if-reply {
  background-color: transparent;
  color: black;
  padding: 0 0;
  border-radius: 10px;
  cursor: pointer;
  font-size: 14px;
  border: none;
}

.comments-section {
  width: 100%;
  /*max-height: 800px;*/
  overflow-y: auto;
  margin-top: 20px;
  margin-bottom: 100px;
  background-color: rgba(255, 255, 255, 0.5);
}

.replying-to {
  margin: 10px 0;
  color: #007bff;
}

textarea {
  width: 100%;
  height: 80px;
  margin-top: 10px;
  padding: 10px;
  border-radius: 5px;
  border: 1px solid #ddd;
  font-size: 14px;
  resize: none;
}

.input-container {
  position: absolute;
  width: 100%;
}

.actions {
  position: absolute;
  bottom: 10px;
  right: 10px;
  display: flex;
  gap: 10px;
}

.emoji-button {
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
  padding: 0;
  position: relative;
}

.btn-primary {
  background-color: #007bff;
  color: #fff;
  padding: 8px 16px;
  border-radius: 5px;
  border: none;
  cursor: pointer;
  font-size: 14px;
}

.back-button {
  position: absolute;
  top: 20px;
  left: 20px;
  background-color: #FFFAFA;
  border-radius: 10px;
  border: none;
  font-size: 30px;
  cursor: pointer;
  padding: 1px 10px;
  transition: background-color 0.3s ease;
}

.back-button:hover {
  background-color: #33ff33;
}

.card {
  margin-top: 65px;
  margin-right: 1%;
  width: 300px;
  background-color: rgba(255, 255, 255, 0.5);
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  padding: 15px;
  border-radius: 10px;
  height: max-content;
}

.related-posts-section .title {
  font-size: 16px;
  font-weight: bolder;
  color: #000;
  padding-left: 0;
  text-align: left;
}

.related-post-item {
  display: flex;
  align-items: center;
  margin: 10px 0;
  cursor: pointer;
}

.related-post-title {
  font-size: 14px;
  color: #007bff;
  margin-left: 8px;
  flex-grow: 1;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  text-align: left;
}

.related-post-title:hover {
  text-decoration: underline;
}

.related-post-divider {
  margin: 0;
  border-top: 1px solid #ddd;
  margin-bottom: 10px;
}

.hot-posts-section .title {
  font-size: 16px;
  font-weight: bolder;
  color: #000;
  padding-left: 0;
  text-align: left;
}


.hot-post-title {
  font-size: 14px;
  color: #007bff;
  margin-left: 8px;
  flex-grow: 1;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  text-align: left;
}


.hot-post-title:hover {
  text-decoration: underline;
}

.post-divider {
  margin: 0;
  border-top: 1px solid #ddd;
  margin-bottom: 20px;
  margin-top: 20px;
  /* 添加空隙 */
}


.row {
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: left;
  margin-left: 20px;
}

.right-sidebar {
  margin-top: 65px;
  margin-right: 1%;
  width: 300px;
  background-color: rgba(255, 255, 255, 0.5);
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  padding: 15px;
  border-radius: 10px;
  height: max-content;
}

.hot-posts-section .title {
  font-size: 18px;
  font-weight: bold;
  color: #000;
  padding-left: 10px;
  text-align: left;
}

.hot-post-item {
  display: flex;
  align-items: center;
  margin: 10px 0;
  cursor: pointer;
}

.hot-post-title {
  font-size: 14px;
  color: #007bff;
  margin-left: 8px;
  flex-grow: 1;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.hot-post-title:hover {
  text-decoration: underline;
}

.icon-link-small,
.icon-fire-small {
  font-size: 15px !important;
  width: 15px !important;
  height: 15px !important;
  margin-right: 8px;
  display: inline-block;
  line-height: 1;
  /* 确保图标的高度不会因为行高影响 */
}

.hot-post-divider {
  margin: 0;
  border-top: 1px solid #ddd;
  margin-bottom: 10px;
}


.fixed-input {
  width: 50%;
  max-height: 300px;
  overflow-y: auto;
  margin-top: 20px;
  background-color: rgba(255, 255, 255, 0.5);
  position: fixed;
  bottom: 50px;
  left: 50%;
  transform: translateX(-50%);
  /*width: 800px;
  /* 与 post-container 的宽度一致 */
  z-index: 101;
  /* 确保在其他元素之上 */
  background-color: transparent;
  padding: 0;
}

.post-image {
  text-align: center;
  /* 图片居中显示 */
  margin-bottom: 20px;
  /* 图片和内容之间的间距 */
  max-height: 300px;

}

.post-image .image {
  max-width: 100%;
  height: 100%;
  border-radius: 5px;
}

.post-content {
  text-align: left;
  /* 内容靠左显示 */
  margin-top: 0;
  /* 去除顶部间距 */
  padding: 10px 0;
  /* 为内容部分添加上下间距 */
}

.back-button-container {
  position: absolute;
  top: 1vh;
  /* 调整为你需要的上边距 */
  left: 5vw;
  /* 调整为你需要的左边距 */
  z-index: 1000;


}

.backHome-button-container {
  position: absolute;
  top: 1vh;
  /* 调整为你需要的上边距 */
  left: 1vw;
  /* 调整为你需要的左边距 */
  z-index: 1000;
  /* 确保按钮在日历表之上 */
}


.backtop-button {
  position: fixed;
  bottom: 180px !important;
  right: 20px !important;
  z-index: 2;
  width: 60px !important;
  /* 增加按钮的宽度 */
  height: 60px !important;
  /* 增加按钮的高度 */
  display: flex;
  justify-content: center;
  align-items: center;
  transition: transform 0.5s ease;
  /* 添加缩放的过渡效果 */
}

.post-content-container {
  background: rgba(255, 255, 255, 0.5);
  /* 白色透明度0.5 */
  border-radius: 12px;
  /* 圆角 */
  backdrop-filter: blur(10px);
  /* 磨砂感 */
  max-height: 500px;
  /* 最大高度，根据需要调整 */
  overflow: auto;
  /* 超出内容显示滚动条 */
  padding: 15px;
  /* 内边距 */
}

.toggle-container-button {
  position: fixed;
  bottom: 60px;
  /* 让按钮位于容器上方 */
  left: 20%;
  transform: translateX(-50%);
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 20px;
  padding: 10px 20px;
  cursor: pointer;
  z-index: 1001;
  /* 确保按钮在容器之上 */
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.5);
  opacity: 1;
  /* 初始完全不透明 */
  transition: opacity 0.5s ease, transform 0.5s ease;
  /* 控制透明度和位移的过渡效果 */
}

/* 点击时的缩放效果 */
.toggle-container-button:active {
  transform: translateX(-50%) scale(0.95);
  /* 点击时缩小按钮 */
  background-color: #0056b3;
  /* 点击时按钮颜色变深 */
  transition: transform 0.1s ease, background-color 0.1s ease;
}


.actions-container {
  position: fixed;
  left: 50%;
  /* 定位到页面的水平中心 */
  bottom: 0;
  /* 定位到页面的底部 */
  transform: translateX(-50%);
  /* 将容器向左移动自身宽度的50%，以实现居中 */
  background: rgba(255, 255, 255, 0.8);
  /* 半透明背景，确保背景与内容分离 */
  backdrop-filter: blur(10px);
  /* 磨砂效果 */
  padding: 15px;
  border-radius: 10px 10px 0 0;
  /* 只为顶部添加圆角 */
  box-shadow: 0 -2px 10px rgba(0, 0, 0, 0.5);
  /* 在顶部添加阴影，使其从页面底部凸出 */
  width: 700px;
  /* 宽度设为100%，以适应移动设备 */
  height: 150px;
  /*max-width: 600px; !* 限制最大宽度 *!*/
  z-index: 1000;
  /* 确保容器在页面的其他内容上方 */
}

.input-container,
.post-actions {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-bottom: 10px;
  width: 100%;
  /* 保证内容宽度占满父容器 */
}
</style>
