<template>
    <div class="comment-item">
        <p>
            <strong :class="{ 'current-user': isCurrentUser(comment.userName) }" style="font-weight: bold">{{ comment.userName }}:</strong>
        </p>
        <div class="comment-content-container">
              <span v-html="renderMarkdown(comment.content)"></span>
        </div>
        <el-text class="comment-time">{{ formatDate(comment.commentTime) }}</el-text>
        <div class="comment-actions">
            <span @click="likeComment(comment.commentID)" @mouseover="highlightCommentAction"
                @mouseleave="resetCommentAction">
                👍 {{ comment.likedByCurrentUser ? '取消' : '点赞' }} {{ comment.likesCount }}
            </span>
            <span @click="setReplyTarget(comment)" @mouseover="highlightCommentAction" @mouseleave="resetCommentAction">
                回复
            </span>
            <span v-if="isCurrentUser(comment.userName)" @click="deleteComment(comment.commentID)">删除</span>
            <button @click="toggleReplies" class="btn-if-reply">
                {{ showReplies ? '隐藏回复↑' : '显示回复↓' }}
            </button>
        </div>

        <div v-if="showReplies">
            <CommentItem v-for="reply in comment.replies" :key="reply.commentID" :comment="reply"
                @fetch-replies="$emit('fetch-replies', $event)" @like-comment="$emit('like-comment', $event)"
                @set-reply-target="$emit('set-reply-target', $event)"
                @delete-comment="$emit('delete-comment', $event)" />
        </div>
    </div>
</template>

<script>
import MarkdownIt from 'markdown-it';
export default {
    name: 'CommentItem',
    props: {
        comment: {
            type: Object,
            required: true,
        },
    },
    data() {
        return {
            showReplies: false,
            currentUser: localStorage.getItem('name'),
            md: new MarkdownIt()
        };
    },
    methods: {
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
        renderMarkdown(content) {
          return this.md.render(content);
        },
        toggleReplies() {
            this.showReplies = !this.showReplies;
            if (this.showReplies && this.comment.replies.length === 0) {
                this.$emit('fetch-replies', this.comment);
            }
        },
        likeComment(commentID) {
            this.$emit('like-comment', commentID);
        },
        setReplyTarget(comment) {
            this.$emit('set-reply-target', comment);
        },
        deleteComment(commentID) {
            this.$emit('delete-comment', commentID);
        },
        highlightCommentAction(event) {
            event.target.style.backgroundColor = '#f0f0f0';
            event.target.style.cursor = 'pointer';
        },
        resetCommentAction(event) {
            event.target.style.backgroundColor = '';
            event.target.style.cursor = '';
        },
    },
};
</script>

<style scoped>
.comment-item {
    margin-bottom: 5px;
    padding: 10px;
    background-color: #f9f9f9;
    border-radius: 5px;
    border: none;
    text-align: left;
}

.current-user {
    color: red;
    font-weight: bold;
}

.comment-actions {
    margin-top: 10px;
    display: flex;
    gap: 10px;
    font-size: 14px;
    color: #555;
}

.comment-actions span:hover {
    cursor: pointer;
    background-color: #f0f0f0;
}

.comment-time {
    margin-left: 0;
    font-size: 12px;
    color: #999;
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

.comment-content-container {
  padding-left: 20px; /* 调整左边距 */
  margin-top: 10px;   /* 调整顶部间距 */
  line-height: 1.6;   /* 调整文本的行高 */
}
</style>
