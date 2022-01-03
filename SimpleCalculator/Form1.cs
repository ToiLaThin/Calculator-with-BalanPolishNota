// Nguon tham khao: https://www.tutorialspoint.com/Convert-Infix-to-Postfix-Expression
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleCalculator
{
    public partial class Form1 : Form
    {
        bool btnEqualClicked;
        public Form1()
        {
            InitializeComponent();
            btnEqualClicked = false;
        }

        #region Event Handler for functional buttons
        //this method used for operator button too
        private void btnNum_Click(object sender, EventArgs e)
        {
            if(!btnEqualClicked)
            {
                txtbDisplayScreen.Text += (sender as Button).Text;
                return;
            }

            btnEqualClicked = false;      
            //xoa man hinh
            btnReset_Click(sender, e);
            txtbDisplayScreen.Text += (sender as Button).Text;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (btnEqualClicked)
                btnEqualClicked = false;
            txtbDisplayScreen.Clear();
        }

        private int numCharToDel(string str)
        {
            int numCharToDel = 0;
            int nowIdx = str.Length - 1;
            char nowChar = str[nowIdx];

            if (nowChar < '0' || nowChar > '9')//+ - * / ( )
                return 1;
            else{
                while (nowChar >= '0' && nowChar <= '9')
                {
                    numCharToDel++;
                    nowIdx--;
                    if (nowIdx < 0)
                        break;
                    nowChar = txtbDisplayScreen.Text[nowIdx];
                }
            }
            return numCharToDel;
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            if (btnEqualClicked)//chi co 1 so 
            {
                txtbDisplayScreen.Clear();
                return;
            }
            else
            {
                if(txtbDisplayScreen.Text.Length>0)
                {
                    int numToDel = numCharToDel(txtbDisplayScreen.Text);
                    //dung substring vi remove bi exception
                    if (txtbDisplayScreen.Text.Length >= numToDel)
                        txtbDisplayScreen.Text = txtbDisplayScreen.Text.Substring(0, txtbDisplayScreen.Text.Length - numToDel);
                }
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtbDisplayScreen.Text);
        }       

        private void btnEqual_Click(object sender, EventArgs e)
        {
            try
            {
                string postfix = toPostfix(txtbDisplayScreen.Text);
                //neu nhap syntax sai thi trong ham calculatedResult se ko the pop 2 num dc => err
                txtbDisplayScreen.Text = calculatedResult(postfix).ToString();
                btnEqualClicked = true;
                return;
            }
            catch (Exception err)
            {
                MessageBox.Show("Syntax Error!!!");
                btnReset_Click(sender, e);
            }
        }
        #endregion     
      
        #region Balan Polish Notation methods

        private int precedenceOf(char ope)
        {
            if (ope == '*' || ope == '/')
                return 2;
            else if (ope == '+' || ope == '-')
                return 1;
            else
                return 0;
            //( return 0 nhung precedence cua no thuc ra la lon nhat theo ly thuyet
        }

        private string toPostfix(string infix)
        {
            string postfix = "";
            Stack<int> st = new Stack<int>();

            for (int i = 0; i < infix.Length; i++)
            {
                if (infix[i] >= '0' && infix[i] <= '9')
                {//TH ki tu la toan hang
                    postfix = postfix + infix[i];
                }
                else if (infix[i] == '(')
                {
                    st.Push(infix[i]);
                }
                else if (infix[i] == ')')
                {
                    postfix += " ";//them ' ' truoc khi cong toan tu dc pop ra vao chuoi
                    while (st.Peek() != '(' && st.Count > 0)
                    {//lay toan tu ra bo vao postfix cho toi khi gap  '(' trong stack
                        postfix = postfix + (char)st.Peek() + " ";
                        st.Pop();
                    }
                    st.Pop();//lay ra '('
                }
                else
                {//TH ki tu la toan tu
                    if (postfix[postfix.Length - 1] != ' ')
                        postfix += " ";
                   
                    if (st.Count==0 || precedenceOf(infix[i]) > precedenceOf((char)st.Peek()))
                    {
                        st.Push(infix[i]);
                    }
                    else
                    {
                        while (st.Count > 0 && precedenceOf(infix[i]) <= precedenceOf((char)st.Peek()))
                        {
                            postfix = postfix + (char)st.Peek() + " ";
                            st.Pop();
                        }
                        st.Push(infix[i]);
                    }
                }

            }

            if (st.Count > 0 && (postfix[postfix.Length - 1] != ' '))
                postfix += " ";
            while (st.Count > 0)
            {
                postfix = postfix + (char)st.Peek() + " ";
                st.Pop();
            }

            return postfix;
        }

        private double resultOfOperation(char ope, double num1, double num2)
        {
            switch (ope)
            {
                case '+': return num1 + num2;
                case '-': return num1 - num2;
                case '*': return num1 * num2;
                case '/': return num1 / num2;
                default: return 0;
            }
        }

        private double calculatedResult(string postfix)
        {
            Stack<double> st = new Stack<double>();
            string aNum = "";
            for (int i = 0; i < postfix.Length; i++)
                if (postfix[i] == '+' || postfix[i] == '-' || postfix[i] == '*' || postfix[i] == '/')
                {
                    double num2 = st.Pop(); double num1 = st.Pop();
                    st.Push(resultOfOperation(postfix[i], num1, num2));
                }
                else if ((postfix[i] == ' ') && (postfix[i - 1] >= '0' && postfix[i - 1] <= '9'))
                {
                    double temp = Convert.ToDouble(aNum);
                    st.Push(temp);
                    aNum = "";
                }
                else if (postfix[i] >= '0' && postfix[i] <= '9')
                {
                    aNum += postfix[i];
                }

            return st.Peek();
        }
        #endregion  

    }
}

